using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using TypingAnalyzer.Core;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Reporters
{
    public class KeyStatsReporter : KeyReporter
    {
        private readonly ILogger<KeyStatsReporter> _logger;
        private readonly IConfiguration _config;
        private readonly string _statsFile;

        private readonly ConcurrentDictionary<Keys, int> _keyCounts = new ConcurrentDictionary<Keys, int>();
        private readonly ConcurrentDictionary<string, int> _wordCounts = new ConcurrentDictionary<string, int>();

        public KeyStatsReporter(ILogger<KeyStatsReporter> logger, IConfiguration config, IKeyProcessor processor) : base(processor)
        {
            _logger = logger;
            _config = config;
            _statsFile = _config.GetValue("Reporting:KeyStatsFile", "KeyStats.xml");
        }

        public override void Start()
        {
            _logger.LogDebug($"Loading previous stats from file {_statsFile}");
            // Load in existing stats
            base.Start();
        }

        protected override async Task ReportKeyData(KeyData data)
        {
            _keyCounts.AddOrUpdate(data.Key, 1, (key, count) =>
            {
                count++;
                _logger.LogTrace($"Key {key} has been seen {count} times.");
                return count;
            });
        }

        protected override async Task ReportCompletedWord(CompletedWord word)
        {
            _wordCounts.AddOrUpdate(word.Data.ToLower(), 1, (key, count) =>
            {
                count++;
                _logger.LogTrace($"Word {key} has been seen {count} times.");
                return count;
            });

            
        }
    }
}
