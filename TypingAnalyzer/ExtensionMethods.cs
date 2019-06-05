using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypingAnalyzer
{
    public static class ExtensionMethods
    {
        public static bool IsAlphaNumeric(this string input) => input.All(c => char.IsLetterOrDigit(c));

        public static bool IsWhitespace(this string input) => input.All(c => char.IsWhiteSpace(c));

        public static bool IsSymbol(this string input) => input.All(c => char.IsSymbol(c));

        public static bool IsPunctuation(this string input) => input.All(c => char.IsPunctuation(c));
    }
}
