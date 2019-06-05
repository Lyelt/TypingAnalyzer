using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TypingAnalyzer.Core;
using TypingAnalyzer.Interfaces;
using TypingAnalyzer.Keyboard;

namespace TypingAnalyzer
{
    public class Analyzer
    {
        private readonly ILogger<Analyzer> _logger;
        private readonly IConfiguration _config;
        private readonly IGlobalKeyboardHook _hook;
        private readonly IActiveWindowHandler _windowHandler;
        private readonly IKeyProcessor _processor;
        private readonly IEnumerable<IKeyReporter> _reporters;

        public Analyzer(ILogger<Analyzer> logger, 
            IConfiguration config, 
            IGlobalKeyboardHook hook, 
            IActiveWindowHandler windowHandler, 
            IKeyProcessor processor, 
            IEnumerable<IKeyReporter> reporters)
        {
            _logger = logger;
            _config = config;
            _windowHandler = windowHandler;
            _hook = hook;
            _processor = processor;
            _reporters = reporters;
        }

        public void Start()
        {
            _hook.KeyboardPressed += HandleKeypress;

            foreach (var reporter in _reporters)
                reporter.Start();

            _logger.LogDebug($"Analyzer started.");
        }

        public void Stop()
        {
            _hook.KeyboardPressed -= HandleKeypress;
            _logger.LogDebug($"Analyzer stopped.");
        }

        private async void HandleKeypress(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == KeyboardState.KeyUp)
            {
                var window = _windowHandler.GetActiveWindow();
                var key = (Keys)e.VirtualKeyCode;

                if (_config.GetValue("Keypresses:LogKeyPresses", true))
                    _logger.LogTrace($"Received keypress: [{e.PressedKey}] (Key: {key}) while window [{window.HostProcess.ProcessName}] was in focus.");

                await _processor.Feed(new KeyData(key, e.PressedKey, window, DateTime.UtcNow));
            }
        }
    }
}
