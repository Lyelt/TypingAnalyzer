using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TypingAnalyzer.Core;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Reporters
{
    public abstract class KeyReporter : IKeyReporter
    {
        protected readonly IKeyProcessor _processor;
        protected readonly ITargetBlock<KeyData> _keyPressBlock;
        protected readonly ITargetBlock<CompletedWord> _completedWordBlock;

        public KeyReporter(IKeyProcessor processor)
        {
            _processor = processor;
            _keyPressBlock = new ActionBlock<KeyData>(ReportKeyData);
            _completedWordBlock = new ActionBlock<CompletedWord>(ReportCompletedWord);
        }

        public virtual void Start()
        {
            _processor.SubscribeToKeyPresses(_keyPressBlock);
            _processor.SubscribeToWords(_completedWordBlock);
        }

        protected abstract Task ReportKeyData(KeyData data);

        protected abstract Task ReportCompletedWord(CompletedWord data);
    }
}
