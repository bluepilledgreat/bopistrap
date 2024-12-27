using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal static class AppRegistry
    {
        public const string UninstallKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\Bopistrap";

        public static void WriteFileClass(string extension, string value)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            string keyPath = $@"Software\Classes\.{extension}";

            using RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath);
            key.SetValue("", value);
        }

        public static void DeleteFileClass(string extension)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            string keyPath = $@"Software\Classes\.{extension}";
            Registry.CurrentUser.DeleteSubKey(keyPath, throwOnMissingSubKey: false);
        }

        public static void DeleteClass(string extension)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            string keyPath = $@"Software\Classes\{extension}";
            Registry.CurrentUser.DeleteSubKeyTree(keyPath, throwOnMissingSubKey: false);
        }

        public static void WriteLaunchClass(string name, string productName, string path)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            string keyPath = $@"Software\Classes\{name}";
            using RegistryKey mainKey = Registry.CurrentUser.CreateSubKey(keyPath);

            using RegistryKey iconKey = mainKey.CreateSubKey("DefaultIcon");
            iconKey.SetValue("", $"{path},0");

            using RegistryKey shellKey = mainKey.CreateSubKey("shell");
            shellKey.SetValue("", "open");

            using RegistryKey openKey = shellKey.CreateSubKey("open");
            mainKey.SetValue("", $"Open with {productName}"); // Open with Bopimo!

            using RegistryKey commandKey = openKey.CreateSubKey("command");
            commandKey.SetValue("", $"\"{path}\" \"%1\"");
        }

        public static void WriteUrlClass(string name, string path)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            string keyPath = $@"Software\Classes\{name}";
            using RegistryKey mainKey = Registry.CurrentUser.CreateSubKey(keyPath);
            mainKey.SetValue("", $"URL:{name}");
            mainKey.SetValue("URL Protocol", "");

            using RegistryKey commandKey = mainKey.CreateSubKey(@"shell\open\command");
            commandKey.SetValue("", $"\"{path}\" \"%1\"");
        }

        public static void RegisterClasses()
        {
            WriteFileClass("bop", "bop");
            WriteFileClass("bopjson", "bop");
            WriteFileClass("bop_old_godot_3", "bop");

            WriteLaunchClass("bop", "Bopistrap", Paths.Bootstrapper);

            WriteUrlClass("bopimo", Paths.Bootstrapper);
        }

        public static void UpdateVersion()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotImplementedException();

            using (var uninstallKey = Registry.CurrentUser.OpenSubKey(UninstallKey))
            {
                uninstallKey?.SetValue("DisplayVersion", Bootstrapper.Version);
            }
        }

        public static void UpdateVersionSafe()
        {
            try
            {
                UpdateVersion();
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to write version to registry");
                Logger.WriteLine(ex);
            }
        }
    }
}
