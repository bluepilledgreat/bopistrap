using Bopistrap.Enums;
using Bopistrap.Extensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal static class Installer
    {
        /// <summary>
        /// Uninstall key for the official Bopimo launcher
        /// </summary>
        private const string BopimoUninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\8b42e276-a9e1-50bb-aa7f-8995242b7e99";

        public static bool IsInstalled()
        {
            // not linux compatible atm
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            return Registry.CurrentUser.OpenSubKey(AppRegistry.UninstallKey) != null && File.Exists(Paths.Bootstrapper);
        }

        public static bool IsNewerVersion()
        {
            // not linux compatible atm
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            using var uninstallKey = Registry.CurrentUser.OpenSubKey(AppRegistry.UninstallKey);

            object? versionValue = uninstallKey?.GetValue("DisplayVersion");
            if (versionValue is not string version1Str)
                return false;

            if (!BopistrapVersion.TryParse(version1Str, out var version1))
                return true;

            if (!BopistrapVersion.TryParse(Bootstrapper.Version, out var version2))
                return false;

            return version2 > version1;
        }

        private static void DoInstall()
        {
            // not linux compatible atm
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            Paths.CreateDirectories();
            File.Copy(Bootstrapper.ExecutingPath, Paths.Bootstrapper, true);

            using (var uninstallKey = Registry.CurrentUser.CreateSubKey(AppRegistry.UninstallKey))
            {
                uninstallKey.SetValue("DisplayIcon", $"{Paths.Bootstrapper},0");
                uninstallKey.SetValue("DisplayName", "Bopistrap");

                uninstallKey.SetValue("DisplayVersion", Bootstrapper.Version);

                if (uninstallKey.GetValue("InstallDate") is null)
                    uninstallKey.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));

                uninstallKey.SetValue("InstallLocation", Paths.Base);
                uninstallKey.SetValue("NoRepair", 1);
                uninstallKey.SetValue("Publisher", "bluepilledgreat");
                uninstallKey.SetValue("ModifyPath", $"\"{Paths.Bootstrapper}\" -settings");
                uninstallKey.SetValue("QuietUninstallString", $"\"{Paths.Bootstrapper}\" -uninstall -quiet");
                uninstallKey.SetValue("UninstallString", $"\"{Paths.Bootstrapper}\" -uninstall");
            }

            AppRegistry.RegisterClasses();

            Settings.Default.Save();
        }

        private static void DoInstallSafe()
        {
            try
            {
                DoInstall();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex);

                Utils.ShowMessageBox($"An error occured while installing Bopistrap:\n\n{ex.Message}", MessageBoxButton.Ok, MessageBoxIcon.Error, quietFallback: MessageBoxResult.Ok);

                EnvironmentEx.Exit(ExitCode.InstallFailure);
            }
        }

        public static void PromptInstall()
        {
            var result = Utils.ShowMessageBox("Do you want to install Bopistrap?", MessageBoxButton.YesNo, MessageBoxIcon.Information, quietFallback: MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                DoInstallSafe();

                Utils.ShowMessageBox("Bopistrap has been installed.", MessageBoxButton.Ok, MessageBoxIcon.Information, quietFallback: MessageBoxResult.Ok);
            }
        }

        private static void DoUninstall()
        {
            // not linux compatible atm
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            Process[] processes = Process.GetProcessesByName("bopimo_client");

            if (processes.Any())
            {
                var result = Utils.ShowMessageBox("To continue the uninstallation, all instances of the Bopimo! Game Client must be closed. Would you like to close them?", MessageBoxButton.YesNo, MessageBoxIcon.Warning, quietFallback: MessageBoxResult.Yes);

                if (result == MessageBoxResult.No)
                {
                    EnvironmentEx.Exit(ExitCode.Cancelled);
                    return;
                }

                foreach (Process process in processes)
                {
                    process.Kill();
                }
            }

            using RegistryKey? bopimoUninstallKey = Registry.CurrentUser.OpenSubKey(BopimoUninstallKey);

            if (bopimoUninstallKey != null && bopimoUninstallKey.GetValue("DisplayIcon") is string displayIconPath)
            {
                Logger.WriteLine("Found the stock Bopimo installation");

                // regular bopimo is still installed, fall back to that
                // DisplayIcon should a value like C:\Users\user\AppData\Roaming\Bopimo!\Launcher\uninstallerIcon.ico
                string rootPath = Path.GetDirectoryName(Path.GetDirectoryName(displayIconPath)!)!; // we should have C:\Users\user\AppData\Roaming\Bopimo! now 
                string clientPath = Path.Combine(rootPath, "Client", "bopimo_client.exe");
                string launcherPath = Path.Combine(rootPath, "Launcher", "Bopimo! Launcher.exe");

                AppRegistry.WriteLaunchClass("bop", "Bopimo!", clientPath);
                AppRegistry.WriteUrlClass("bopimo", launcherPath);
            }
            else
            {
                Logger.WriteLine("Removing classes from registry");

                // unregister classes
                AppRegistry.DeleteFileClass("bop");
                AppRegistry.DeleteFileClass("bopjson");
                AppRegistry.DeleteFileClass("bop_old_godot_3");

                AppRegistry.DeleteClass("bop");
                AppRegistry.DeleteClass("bopimo");
            }

            Directory.Delete(Paths.Client, true);

            bool deleteFolder = Directory.GetFiles(Paths.Base).Length <= 4; // make sure we're not deleting the entirety of our drive :)
            if (deleteFolder)
            {
                try
                {
                    Directory.Delete(Paths.Base, true);
                }
                catch
                {
                    // we'll try again later.
                }
            }

            Registry.CurrentUser.DeleteSubKey(AppRegistry.UninstallKey);
        }

        private static void StartDirectoryDeletion()
        {
            if (!Directory.Exists(Paths.Base))
                return;

            bool deleteFolder = Directory.GetFiles(Paths.Base).Length <= 4; // make sure we're not deleting the entirety of our drive :)

            string deleteCommand;

            if (deleteFolder)
                deleteCommand = $"del /Q \"{Paths.Base}\\*\" && rmdir \"{Paths.Base}\"";
            else
                deleteCommand = $"del /Q \"{Paths.Bootstrapper}\"";

            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/c timeout 1 && {deleteCommand}",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        public static void PromptUninstall()
        {
            var result = Utils.ShowMessageBox("Are you sure that you want to uninstall Bopistrap?", MessageBoxButton.YesNo, MessageBoxIcon.Warning, quietFallback: MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                DoUninstall();

                Utils.ShowMessageBox("Bopistrap has been uninstalled.", MessageBoxButton.Ok, MessageBoxIcon.Information, quietFallback: MessageBoxResult.Ok);

                StartDirectoryDeletion();

                EnvironmentEx.Exit(ExitCode.ProductUninstalled);
            }
        }

        public static void HandleUpgrade(string[] args)
        {
            // not linux compatible atm
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            // make sure all other bopistrap instances are closed before we move
            _ = Settings.Default; // load settings

            using (Process currentProcess = Process.GetCurrentProcess())
            {
                foreach (Process p in Process.GetProcessesByName("Bopistrap"))
                {
                    if (p != currentProcess)
                        p.Kill();
                }
            }

            Settings.Default.Save();

            // should be safe now
            File.Move(Bootstrapper.ExecutingPath, Paths.Bootstrapper, true);

            AppRegistry.UpdateVersionSafe();

            // we have to relaunch so funny stuff doesn't happen.
            using Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WorkingDirectory = Paths.Base;
            process.StartInfo.FileName = Paths.Bootstrapper;
            foreach (string arg in args)
                process.StartInfo.ArgumentList.Add(arg);
            process.Start();
        }

        public static void PromptUpgrade(string[] args)
        {
            var result = Utils.ShowMessageBox("This version of Bopistrap is newer than the version installed. Do you want to upgrade?", MessageBoxButton.YesNo, MessageBoxIcon.Information, quietFallback: MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                HandleUpgrade(args);
            }
        }
    }
}
