using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TypingAnalyzer.Core;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Reporters
{
    public class ContinuousStreamReporter : KeyReporter
    {
        private readonly ILogger<ContinuousStreamReporter> _logger;
        private readonly IConfiguration _config;
        private readonly string _streamFile;

        public ContinuousStreamReporter(ILogger<ContinuousStreamReporter> logger, IConfiguration config, IKeyProcessor processor) : base(processor)
        {
            _logger = logger;
            _config = config;
            _streamFile = _config.GetValue("Reporting:ContinuousStreamFile", "KeyStream.txt");
        }

        protected override async Task ReportKeyData(KeyData data)
        {
            _logger.LogTrace($"Reporting continuous data [{data.Data}] to file [{_streamFile}]");
            await File.AppendAllTextAsync(_streamFile, data.Data, Encoding.Unicode);
        }
    }
}
