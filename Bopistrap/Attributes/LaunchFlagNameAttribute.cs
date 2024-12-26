using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.Attributes
{
    internal class LaunchFlagNameAttribute : Attribute
    {
        public string Name { get; }

        public LaunchFlagNameAttribute(string name)
        {
            Name = name;
        }
    }
}
