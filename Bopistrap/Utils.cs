using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using System.Runtime.InteropServices;
using Bopistrap.Enums;

namespace Bopistrap
{
    internal static class Utils
    {
        [DllImport("user32.dll")]
        private static extern int MessageBoxA([In] IntPtr hwnd, [In] [MarshalAs(UnmanagedType.LPStr)] string text, [In][MarshalAs(UnmanagedType.LPStr)] string caption, uint type);

        // TODO: create a message box window in Avalonia
        public static MessageBoxResult ShowMessageBox(string message, MessageBoxButton button, MessageBoxIcon image)
        {
            return (MessageBoxResult)MessageBoxA(IntPtr.Zero, message, "Bopistrap", (uint)button | (uint)image);
        }

        public static MessageBoxResult ShowMessageBox(string message, MessageBoxButton button, MessageBoxIcon image, MessageBoxResult quietFallback)
        {
            if (LaunchFlags.Default.Quiet)
                return quietFallback;

            return ShowMessageBox(message, button, image);
        }
    }
}
