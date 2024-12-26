using Bopistrap.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal class LaunchFlags
    {
        public static LaunchFlags Default { get; } = new LaunchFlags();

        public string? LevelPath { get; set; } = null;
        public string? Deeplink { get; set; } = null;

        [LaunchFlagName("settings")]
        public bool Settings { get; set; } = false;

        [LaunchFlagName("uninstall")]
        public bool Uninstall { get; set; } = false;

        [LaunchFlagName("quiet")]
        public bool Quiet { get; set; } = false;

        [LaunchFlagName("upgrade")]
        public bool Upgrade { get; set; } = false;

        public void Parse(string[] args)
        {
            if (!args.Any()) // dont bother
                return;

            Dictionary<string, PropertyInfo> flagMap = [];

            // build flag map
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (prop.PropertyType != typeof(bool))
                    continue;

                LaunchFlagNameAttribute? attribute = prop.GetCustomAttribute<LaunchFlagNameAttribute>();
                if (attribute == null)
                    continue;

                flagMap.Add(attribute.Name, prop);
            }

            Debug.Assert(flagMap.Any());

            foreach (string arg in args)
            {
                string argName = arg;

                if (arg.StartsWith("bopimo://"))
                {
                    Deeplink = arg;
                }
                else if (arg.Length > 2 && arg[1] == ':')
                {
                    LevelPath = arg;
                }
                else if (argName.StartsWith('-'))
                {
                    argName = argName[1..]; // remove -

                    if (flagMap.TryGetValue(argName, out PropertyInfo? prop))
                        prop.SetValue(this, true);
                }
            }
        }
    }
}
