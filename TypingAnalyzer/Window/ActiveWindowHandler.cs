using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Window
{
    public class ActiveWindowHandler : IActiveWindowHandler
    {
        public string GetActiveWindow()
        {
            var buf = new StringBuilder(256);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, buf, 256) > 0)
            {
                return buf.ToString();
            }

            return null;
        }


        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    }
}
