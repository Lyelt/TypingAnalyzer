using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Keyboard
{
    public class GlobalKeyboardHookEventArgs : HandledEventArgs
    {
        public KeyboardState KeyboardState { get; private set; }
        public string PressedKey { get; private set; }
        public char? ModifiedPressedKey { get; private set; }

        public GlobalKeyboardHookEventArgs(string key, char? modifiedKey, KeyboardState keyboardState)
        {
            PressedKey = key;
            ModifiedPressedKey = modifiedKey;
            KeyboardState = keyboardState;
        }
    }
}
