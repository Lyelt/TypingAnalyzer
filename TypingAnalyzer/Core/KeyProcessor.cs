using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Core
{
    public class KeyProcessor : IKeyProcessor
    {
        private readonly IActiveWord _activeWord;
        private readonly IPropagatorBlock<KeyData, KeyData> _incomingKeyPresses;
        private readonly IPropagatorBlock<KeyData, CompletedWord> _incomingCompletedWords;

        public KeyProcessor(IActiveWord activeWord)
        {
            _activeWord = activeWord;
            _incomingKeyPresses = new TransformBlock<KeyData, KeyData>(ProcessKey);
            _incomingCompletedWords = new TransformBlock<KeyData, CompletedWord>(ProcessWord);

            _incomingKeyPresses.LinkTo(_incomingCompletedWords, _ => _activeWord.IsComplete);
            _incomingCompletedWords.LinkTo(DataflowBlock.NullTarget<CompletedWord>());
        }

        public async Task Feed(KeyData keyData) => await _incomingKeyPresses.SendAsync(keyData);

        public void SubscribeToKeyPresses(ITargetBlock<KeyData> target) => _incomingKeyPresses.LinkTo(target);

        public void SubscribeToWords(ITargetBlock<CompletedWord> target) => _incomingCompletedWords.LinkTo(target);

        private KeyData ProcessKey(KeyData keyData)
        {
            if (keyData.Data.IsAlphaNumeric())
                _activeWord.AddAlphaNumeric(keyData);
            else if (keyData.Data.IsWhitespace())
                _activeWord.AddWhitespace(keyData);
            else if (keyData.Data.IsSymbol())
                _activeWord.AddSymbol(keyData);
            else if (keyData.Data.IsPunctuation())
                _activeWord.AddPunctuation(keyData);
            else if (keyData.Key == Keys.Back || keyData.Key == Keys.Delete)
                _activeWord.AddDeletion(keyData);

            return keyData;
        }

        private CompletedWord ProcessWord(KeyData keyData)
        {
            return new CompletedWord(_activeWord.CurrentData, keyData.Window, DateTime.UtcNow);
        }
    }
}
