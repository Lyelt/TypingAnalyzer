using System;
using System.Collections.Generic;
using System.Text;

namespace TypingAnalyzer.Core
{
    public class KeyDataEventArgs : EventArgs
    {
        public KeyData KeyData { get; }

        public KeyDataEventArgs(KeyData keyData)
        {
            KeyData = keyData;
        }
    }

    public class WordTypedEventArgs : KeyDataEventArgs
    {
        public string CompletedWord { get; }

        public WordTypedEventArgs(KeyData keyData, string completedWord) : base(keyData)
        {
            CompletedWord = completedWord;
        }
    }
}
