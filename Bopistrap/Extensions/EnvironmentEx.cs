using Bopistrap.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.Extensions
{
    internal static class EnvironmentEx
    {
        public static void Exit(ExitCode code)
        {
            Environment.Exit((int)code);
        }
    }
}
