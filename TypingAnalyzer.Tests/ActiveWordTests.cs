using NUnit.Framework;
using System;
using System.Windows.Forms;
using TypingAnalyzer.Core;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddLetter_BeginsWord()
        {
            var word = new ActiveWord();

            word.AddAlphaNumeric(new KeyData(Keys.A, "a", null, DateTime.UtcNow));

            Assert.False(word.IsComplete);
            Assert.That(word.CurrentData.Length == 1);
        }
    }
}