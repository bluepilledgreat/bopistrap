using Bopistrap.Enums;
using Bopistrap.Extensions;
using Bopistrap.Models;
using ICSharpCode.SharpZipLib.Zip;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal class Bootstrapper
    {
        private struct ClientRelease
        {
            public string Version;
            public string DownloadUrl;
        }

        private struct FileInfo
        {
            public int Size;

            /// <summary>
            /// HTTP file stream
            /// </summary>
            public Stream Stream;
        }

        /// <summary>
        /// Bopistrap version
        /// </summary>
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

        public static BopistrapVersion AdvancedVersion { get; } = BopistrapVersion.Parse(Version);

        /// <summary>
        /// Path of the executable
        /// </summary>
        public static string ExecutingPath { get; } = Environment.ProcessPath!;

        private const string Domain = "bopimo.com";
        private const string Repository = "bluepilledgreat/bopistrap";

        private const string ClientReleaseApiUrl = $"https://www.{Domain}/api/releases/client/latest";
        private const string GithubReleaseApiUrl = $"https://api.github.com/repos/{Repository}/releases/latest";

        private readonly IBootstrapperDialog _dialog;
        private readonly CancellationToken _token;

        public Bootstrapper(IBootstrapperDialog dialog, CancellationToken token)
        {
            _dialog = dialog;
            _token = token;

            if (!string.IsNullOrEmpty(Settings.Default.State.Version))
                _dialog.GameVersion = $"v{Settings.Default.State.Version}";
        }

        private static ClientPlatform GetClientPlatform()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ClientPlatform.Windows : ClientPlatform.Linux;
        }

        private async Task<ClientRelease> GetLatestClientRelease()
        {
            ClientReleaseResponse response = await Http.Client.GetFromJsonSafeAsync<ClientReleaseResponse>(ClientReleaseApiUrl, _token);

            return new ClientRelease
            {
                Version = response.Version,
                DownloadUrl = response.Files[GetClientPlatform()]
            };
        }

        private async Task<FileInfo> GetFile(string url)
        {
            HttpResponseMessage response = await Http.Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, _token);
            response.EnsureSuccessStatusCode();

            int size = 0;

            if (response.Content.Headers.TryGetValues("Content-Length", out IEnumerable<string>? values))
            {
                string sizeStr = values.First();
                if (!int.TryParse(sizeStr, out size))
                {
                    Logger.WriteLine($"Failed to parse Content-Length: {size}");
                }
            }
            else
            {
                Logger.WriteLine($"Could not find the Content-Length header");
            }

            return new FileInfo
            {
                Size = size,
                Stream = await response.Content.ReadAsStreamAsync(_token)
            };
        }

        private void UpdateProgressBar(int downloadedSize, int fileSize)
        {
            _dialog.ProgressBarValue = (double)downloadedSize / (double)fileSize;
        }

        private async Task DownloadFile(FileInfo fileInfo, string filePath)
        {
            using var httpStream = fileInfo.Stream;
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.Delete);

            byte[] buffer = new byte[4096];
            int downloadedBytes = 0;

            _dialog.ProgressBarVisible = true;

            while (true)
            {
                int bytesRead = await httpStream.ReadAsync(buffer, _token);
                if (bytesRead == 0)
                    break;

                downloadedBytes += bytesRead;

                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), _token);
                UpdateProgressBar(downloadedBytes, fileInfo.Size);
            }

            _dialog.ProgressBarVisible = false;
        }

        private async Task DownloadFile(string url, string filePath)
        {
            FileInfo zipInfo = await GetFile(url);
            await DownloadFile(zipInfo, filePath);
        }

        private void UnpackZip(string zipFilePath, string destinationDirectory)
        {
            FastZipEvents events = new FastZipEvents();
            events.FileFailure += (_, e) => throw e.Exception;
            events.DirectoryFailure += (_, e) => throw e.Exception;
            events.ProcessFile += (_, e) => e.ContinueRunning = !_token.IsCancellationRequested;

            FastZip fastZip = new FastZip(events);

            fastZip.ExtractZip(zipFilePath, destinationDirectory, null);

            _token.ThrowIfCancellationRequested();

            Logger.WriteLine($"Finished extracting zip");
        }

        private static string? GetGameClientArg()
        {
            if (LaunchFlags.Default.LevelPath != null)
                return LaunchFlags.Default.LevelPath;
            else if (LaunchFlags.Default.Deeplink != null)
                return LaunchFlags.Default.Deeplink;

            return null;
        }

        private bool IsVersionNewer(string versionStr)
        {
            if (!BopistrapVersion.TryParse(versionStr, out BopistrapVersion? version))
                return true; // we probably did something funny

            return version > AdvancedVersion;
        }

        private string? GetDownloadUrlFromGithubAssets(GithubAsset[] assets)
        {
            foreach (GithubAsset asset in assets)
            {
                // TODO: linux support
                if (asset.FileName.EndsWith(".exe")) // this is for our os
                    return asset.DownloadUrl;
            }

            return null;
        }

        private async Task UpdateBopistrap()
        {
            _dialog.Message = "Checking for Bopistrap updates...";

            GithubRelease latestRelease;

            try
            {
                latestRelease = await Http.Client.GetFromJsonSafeAsync<GithubRelease>(GithubReleaseApiUrl, _token);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to fetch Bopistrap release information");
                Logger.WriteLine(ex);
                return;
            }

            if (!IsVersionNewer(latestRelease.TagName))
                return;

            string? downloadUrl = GetDownloadUrlFromGithubAssets(latestRelease.Assets);
            if (downloadUrl == null)
            {
                Logger.WriteLine("Failed to get the suitable Bopistrap download url");
                Logger.WriteLine($"Version: {latestRelease.TagName}");
                Logger.WriteLine("Files:");
                foreach (GithubAsset asset in latestRelease.Assets)
                    Logger.WriteLine(asset.FileName);

                return;
            }

            _dialog.Message = "Downloading the latest Bopistrap...";
            await DownloadFile(downloadUrl, Paths.BootstrapperUpdate);

            _dialog.Message = "Updating Bopistrap...";

            using Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WorkingDirectory = Paths.Base;
            process.StartInfo.FileName = Paths.BootstrapperUpdate;
            foreach (string arg in Program.Arguments)
                process.StartInfo.ArgumentList.Add(arg);
            process.StartInfo.ArgumentList.Add("-upgrade");
            process.Start();

            Environment.Exit(0);
        }

        private async Task UpdateBopimo()
        {
            _dialog.Message = "Checking for Bopimo! client updates...";

            ClientRelease release = await GetLatestClientRelease();
            if (Settings.Default.State.Version == release.Version)
                return;

            _dialog.Message = "Downloading the Bopimo! Game Client...";
            _dialog.GameVersion = $"v{release.Version}";

            string tempFilePath = Path.GetTempFileName();

            try
            {
                await DownloadFile(release.DownloadUrl, tempFilePath);

                _dialog.Message = "Unpacking the Bopimo! Game Client...";

                UnpackZip(tempFilePath, Paths.Client);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Caught an exception while downloading/unpacking client");
                Logger.WriteLine(ex);

                try
                {
                    File.Delete(tempFilePath);
                }
                catch
                {
                    // do nothing
                }

                throw;
            }

            Settings.Default.State.Version = release.Version;

            File.Delete(tempFilePath);
        }

        private void StartBopimo()
        {
            _dialog.Message = "Launching Bopimo!";

            using Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WorkingDirectory = Paths.Client;
            process.StartInfo.FileName = Path.Combine(Paths.Client, "bopimo_client.exe");

            string? arg = GetGameClientArg();
            if (arg != null)
                process.StartInfo.ArgumentList.Add(arg);

            process.Start();
        }

        public async Task Run()
        {
            await UpdateBopistrap();
            await UpdateBopimo();
            StartBopimo();
        }
    }
}
