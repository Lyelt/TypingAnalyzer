using System;
using System.Collections.Generic;
using System.Text;
using TypingAnalyzer.Core;

namespace TypingAnalyzer.Interfaces
{
    public interface IActiveWord
    {
        bool IsComplete { get; }
        
        string CurrentData { get; }

        void AddAlphaNumeric(KeyData data);

        void AddSymbol(KeyData data);

        void AddPunctuation(KeyData data);

        void AddWhitespace(KeyData data);

        void AddDeletion(KeyData data);
    }
}
