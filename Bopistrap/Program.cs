using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Bopistrap.Enums;
using Bopistrap.UI.Bootstrappers;
using Bopistrap.UI.Windows;

namespace Bopistrap
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        //[STAThread]
        //public static void Main(string[] args) => BuildAvaloniaApp()
        //    .StartWithClassicDesktopLifetime(args);

        public static string[] Arguments { get; private set; } = null!;

        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

        private static CancellationTokenSource _bootstrapperTokenSource = new CancellationTokenSource();

        public static void StopBootstrapper()
        {
            _bootstrapperTokenSource.Cancel();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            bool quiet = false;

            try
            {
                quiet = LaunchFlags.Default.Quiet;
            }
            catch { }

            if (!quiet)
            {
                Exception ex = (Exception)e.ExceptionObject;

                Logger.WriteLine(ex);
                Utils.ShowMessageBox($"An exception has occured!\n\n{ex.Message}", MessageBoxButton.Ok, MessageBoxIcon.Error);
            }
        }

        private static void WaitForEnd(Application app, Window window, Task task)
        {
            var cts = new CancellationTokenSource();

            window.Closed += (_, _) => cts.Cancel();
            Task continueTask = task.ContinueWith(x =>
            {
                cts.Cancel();

                if (x.IsFaulted)
                    throw x.Exception;
            });

            if (!window.IsVisible)
                window.Show();

            app.Run(cts.Token);
            
            // make sure the bootstrapper take is also done
            _bootstrapperTokenSource.Cancel();
            continueTask.Wait();
        }

        private static void AppMain(Application app, string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Arguments = args;

            Logger.WriteLine($"Bopistrap v{Bootstrapper.Version}");
            LaunchFlags.Default.Parse(args);

            BopistrapVersion.PerformTests();

            if (!Installer.IsInstalled())
            {
                Installer.PromptInstall();
                return;
            }

            Paths.CreateDirectories();

            if (LaunchFlags.Default.Uninstall)
            {
                Installer.PromptUninstall();
                return;
            }

            if (LaunchFlags.Default.Upgrade)
            {
                Installer.HandleUpgrade();
                return;
            }

            if (Installer.IsNewerVersion() && Path.GetDirectoryName(Bootstrapper.ExecutingPath) != Paths.Base)
                Installer.PromptUpgrade();

            AppRegistry.UpdateVersionSafe();

            // make sure they're registered
            AppRegistry.RegisterClasses();

            if (LaunchFlags.Default.Settings)
            {
                app.RunWithMainWindow<SettingsWindow>();
            }
            else
            {
                if (!LaunchFlags.Default.Quiet)
                {
                    BopimoWindow window = new BopimoWindow();
                    WaitForEnd(app, window, Task.Run(new Bootstrapper(window, _bootstrapperTokenSource.Token).Run));
                }
                else
                {
                    Task.Run(new Bootstrapper(new DummyWindow(), _bootstrapperTokenSource.Token).Run).ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                            throw x.Exception;
                    }).Wait();
                }
            }

            Settings.Default.Save();
        }
    }
}
