using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Window
{
    public class ActiveWindowHandler : IActiveWindowHandler
    {
        public ActiveWindow GetActiveWindow()
        {
            var description = new StringBuilder(256);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, description, 256) > 0)
            {
                GetWindowThreadProcessId(handle, out uint pid);
                var process = Process.GetProcessById((int)pid);

                return new ActiveWindow(process, description.ToString());
            }

            return null;
        }

        // The GetForegroundWindow function returns a handle to the foreground window
        // (the window  with which the user is currently working).
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        // The GetWindowThreadProcessId function retrieves the identifier of the thread
        // that created the specified window and, optionally, the identifier of the
        // process that created the window.
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }
}
