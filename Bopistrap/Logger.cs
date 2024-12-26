using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal static class Logger
    {
        public static void WriteLine(object? message)
        {
            Debug.WriteLine(message);
        }
    }
}
