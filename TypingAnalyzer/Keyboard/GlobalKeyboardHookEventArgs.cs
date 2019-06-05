using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Keyboard
{
    public class GlobalKeyboardHookEventArgs : HandledEventArgs
    {
        public int VirtualKeyCode { get; }

        public KeyboardState KeyboardState { get; }

        public string PressedKey { get; }

        public GlobalKeyboardHookEventArgs(int vKeyCode, string key, KeyboardState keyboardState)
        {
            VirtualKeyCode = vKeyCode;
            PressedKey = key;
            KeyboardState = keyboardState;
        }
    }
}
