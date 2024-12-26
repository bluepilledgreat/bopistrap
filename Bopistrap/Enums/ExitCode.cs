using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.Enums
{
    // https://i-logic.com/serial/errorcodes.htm
    // https://learn.microsoft.com/en-us/windows/win32/msi/error-codes
    internal enum ExitCode
    {
        /// <summary>
        /// The operation was canceled by the user.
        /// </summary>
        Cancelled = 1223,

        /// <summary>
        /// The user canceled installation.
        /// </summary>
        InstallUserExit = 1602,

        /// <summary>
        /// A fatal error occurred during installation.
        /// </summary>
        InstallFailure = 1603,

        /// <summary>
        /// The product is uninstalled.
        /// </summary>
        ProductUninstalled = 1614
    }
}
