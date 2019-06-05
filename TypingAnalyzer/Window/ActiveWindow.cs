using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TypingAnalyzer.Window
{
    public class ActiveWindow
    {
        public string WindowDescription { get; }

        public Process HostProcess { get; }

        public ActiveWindow(Process process, string desc)
        {
            HostProcess = process;
            WindowDescription = desc;
        }
    }
}
