using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TypingAnalyzer.Window;

namespace TypingAnalyzer.Core
{
    public class KeyData
    {
        public Keys Key { get; }

        public string Data { get; }

        public ActiveWindow Window { get; }

        public DateTime TimeOfPress { get; }

        public KeyData(Keys key, string pressedKey, ActiveWindow window, DateTime time)
        {
            Key = key;
            Data = pressedKey;
            Window = window;
            TimeOfPress = time;
        }
    }

    public class CompletedWord
    {
        public string Data { get; }

        public ActiveWindow Window { get; }

        public DateTime TimeOfCompletion { get; }

        public CompletedWord(string data, ActiveWindow window, DateTime time)
        {
            Data = data;
            Window = window;
            TimeOfCompletion = time;
        }
    }
}
