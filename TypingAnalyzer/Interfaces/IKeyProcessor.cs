using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TypingAnalyzer.Core;

namespace TypingAnalyzer.Interfaces
{
    public interface IKeyProcessor
    {
        Task Feed(KeyData data);

        void SubscribeToKeyPresses(ITargetBlock<KeyData> target);

        void SubscribeToWords(ITargetBlock<CompletedWord> target);
    }
}
