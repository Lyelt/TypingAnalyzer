﻿using System;
using System.Collections.Generic;
using System.Text;
using TypingAnalyzer.Keyboard;

namespace TypingAnalyzer.Interfaces
{
    public interface IKeyboardEventArgs
    {
        char KeyboardData { get; }
    }
}
