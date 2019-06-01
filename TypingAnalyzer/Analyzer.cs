using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public Analyzer(ILogger<Analyzer> logger, IConfiguration config, IGlobalKeyboardHook hook, IActiveWindowHandler windowHandler)
        {
            _logger = logger;
            _config = config;
            _windowHandler = windowHandler;
            _hook = hook;
        }

        public void Start()
        {
            _hook.KeyboardPressed += HandleKeypress;
            _logger.LogDebug($"Analyzer started.");
        }

        public void Stop()
        {
            _hook.KeyboardPressed -= HandleKeypress;
            _logger.LogDebug($"Analyzer stopped.");
        }

        private void HandleKeypress(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == KeyboardState.KeyUp)
            {
                if (_config.GetValue("Keypresses:LogKeyPresses", true))
                    _logger.LogTrace($"Received keypress: [{e.PressedKey}] while window [{_windowHandler.GetActiveWindow()}] was in focus.");
            }
        }
    }
}
