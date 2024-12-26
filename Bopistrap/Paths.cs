using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal static class Paths
    {
        public static string Base { get; }
        public static string Client { get; }

        public static string Settings { get; }
        public static string Bootstrapper { get; }
        public static string BootstrapperUpdate { get; }

        public static void CreateDirectories()
        {
            Directory.CreateDirectory(Base);
            Directory.CreateDirectory(Client);
        }

        static Paths()
        {
            Base = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bopistrap");
            Client = Path.Combine(Base, "Client");

            Settings = Path.Combine(Base, "Settings.json");
            Bootstrapper = Path.Combine(Base, "Bopistrap.exe");
            BootstrapperUpdate = Path.Combine(Base, "Bopistrap.update.exe");
        }
    }
}
