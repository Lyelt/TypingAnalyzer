using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TypingAnalyzer.Core;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Reporters
{
    public class KeyStatsReporter : KeyReporter
    {
        private readonly ILogger<KeyStatsReporter> _logger;

        public KeyStatsReporter(ILogger<KeyStatsReporter> logger, IKeyProcessor processor) : base(processor)
        {
            _logger = logger;

            var x = new ActionBlock<KeyData>(ReportKeyData);
        }

        public override void Start()
        {
            // Load in existing stats
            base.Start();
        }

        protected override async Task ReportKeyData(KeyData data)
        {

        }

        protected override async Task ReportCompletedWord(CompletedWord word)
        {

        }
    }
}
