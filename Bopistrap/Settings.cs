using Bopistrap.Enums;
using Bopistrap.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal class Settings
    {
        public class UserSettings
        {
            public BootstrapperStyle BootstrapperStyle { get; set; } = BootstrapperStyle.Red;
        }

        public class StateSettings
        {
            public string Version { get; set; } = "";
        }

        public static Settings Default { get; } = Read();

        public UserSettings User { get; set; } = new UserSettings();
        public StateSettings State { get; set; } = new StateSettings();

        private static Settings Read()
        {
            if (!File.Exists(Paths.Settings))
                return new Settings();

            try
            {
                return JsonSerializerEx.DeserializeSafe<Settings>(File.ReadAllText(Paths.Settings));
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to load Settings file");
                Logger.WriteLine(ex);

                return new Settings();
            }
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(Paths.Settings, JsonSerializer.Serialize(this));
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to save Settings file");
                Logger.WriteLine(ex);
            }
        }
    }
}
