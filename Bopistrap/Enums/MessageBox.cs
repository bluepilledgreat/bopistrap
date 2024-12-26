using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.Enums
{
    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messageboxa
    internal enum MessageBoxButton : uint
    {
        AbortRetryIgnore = 0x2,
        CancelTryContinue = 0x6,
        Help = 0x4000,
        Ok = 0x0,
        OkCancel = 0x1,
        RetryCancel = 0x5,
        YesNo = 0x4,
        YesNoCancel = 0x3
    }

    internal enum MessageBoxIcon : uint
    {
        Exclamation = 0x30,
        Warning = 0x30,
        Information = 0x40,
        Asterisk = 0x40,
        Question = 0x20,
        Stop = 0x10,
        Error = 0x10,
        Hand = 0x10
    }

    internal enum MessageBoxResult : int
    {
        Abort = 3,
        Cancel = 2,
        Continue = 11,
        Ignore = 5,
        No = 7,
        Ok = 1,
        Retry = 4,
        TryAgain = 10,
        Yes = 6
    }
}
