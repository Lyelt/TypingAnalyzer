using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TypingAnalyzer.Interfaces;

namespace TypingAnalyzer.Core
{
    public class ActiveWord : IActiveWord
    {
        public string CurrentData { get; private set; } = string.Empty;
        public bool IsComplete { get; private set; } = false;

        private readonly StateMachine<WordState, KeypressTrigger> _stateMachine;
        private readonly StateMachine<WordState, KeypressTrigger>.TriggerWithParameters<KeyData> _alphaNumericTrigger;
        private readonly StateMachine<WordState, KeypressTrigger>.TriggerWithParameters<KeyData> _symbolTrigger;
        private readonly StateMachine<WordState, KeypressTrigger>.TriggerWithParameters<KeyData> _punctuationTrigger;
        private readonly StateMachine<WordState, KeypressTrigger>.TriggerWithParameters<KeyData> _whitespaceTrigger;
        private readonly StateMachine<WordState, KeypressTrigger>.TriggerWithParameters<KeyData> _deletionTrigger;

        public ActiveWord()
        {
            _stateMachine = new StateMachine<WordState, KeypressTrigger>(WordState.NotStarted);
            _alphaNumericTrigger = _stateMachine.SetTriggerParameters<KeyData>(KeypressTrigger.Alphanumeric);
            _symbolTrigger = _stateMachine.SetTriggerParameters<KeyData>(KeypressTrigger.Symbol);
            _punctuationTrigger = _stateMachine.SetTriggerParameters<KeyData>(KeypressTrigger.Punctuation);
            _whitespaceTrigger = _stateMachine.SetTriggerParameters<KeyData>(KeypressTrigger.Whitespace);
            _deletionTrigger = _stateMachine.SetTriggerParameters<KeyData>(KeypressTrigger.Deletion);
            
            ConfigureStateMachine();
        }

        private void ConfigureStateMachine()
        {
            _stateMachine.Configure(WordState.NotStarted)
                .Ignore(KeypressTrigger.Deletion)
                .Ignore(KeypressTrigger.Punctuation)
                .Ignore(KeypressTrigger.Whitespace)
                .OnEntryFrom(_deletionTrigger, data => ResetWord())
                .Permit(KeypressTrigger.Alphanumeric, WordState.Started)
                .Permit(KeypressTrigger.Symbol, WordState.Started);

            _stateMachine.Configure(WordState.Started)
                .OnEntryFrom(_alphaNumericTrigger, data => OnAlphanumericTyped(data))
                .OnEntryFrom(_symbolTrigger, data => OnSymbolTyped(data))
                .PermitReentry(KeypressTrigger.Alphanumeric)
                .PermitReentry(KeypressTrigger.Symbol)
                .Permit(KeypressTrigger.Punctuation, WordState.Completed)
                .Permit(KeypressTrigger.Whitespace, WordState.Completed)
                .Permit(KeypressTrigger.Deletion, WordState.Interrupted);

            _stateMachine.Configure(WordState.Completed)
                .OnEntryFrom(_punctuationTrigger, data => OnWordCompleted(data))
                .OnEntryFrom(_whitespaceTrigger, data => OnWordCompleted(data))
                .Permit(KeypressTrigger.Alphanumeric, WordState.Started)
                .Permit(KeypressTrigger.Symbol, WordState.Started)
                .Permit(KeypressTrigger.Deletion, WordState.NotStarted)
                .Permit(KeypressTrigger.Whitespace, WordState.NotStarted)
                .Permit(KeypressTrigger.Punctuation, WordState.NotStarted);

            _stateMachine.Configure(WordState.Interrupted)
                .OnEntryFrom(_deletionTrigger, data => OnDeletionTyped(data))
                .PermitReentry(KeypressTrigger.Deletion)
                .Permit(KeypressTrigger.Alphanumeric, WordState.Started)
                .Permit(KeypressTrigger.Symbol, WordState.Started)
                .Permit(KeypressTrigger.Whitespace, WordState.NotStarted)
                .Permit(KeypressTrigger.Punctuation, WordState.NotStarted);
        }

        public void AddAlphaNumeric(KeyData data)
        {
            _stateMachine.Fire(_alphaNumericTrigger, data);
        }

        public void AddSymbol(KeyData data)
        {
            _stateMachine.Fire(_symbolTrigger, data);
        }

        public void AddWhitespace(KeyData data)
        {
            _stateMachine.Fire(_whitespaceTrigger, data);
        }

        public void AddPunctuation(KeyData data)
        {
            _stateMachine.Fire(_punctuationTrigger, data);
        }

        public void AddDeletion(KeyData data)
        {
            _stateMachine.Fire(_deletionTrigger, data);
        }
        
        private void ResetWord()
        {
            CurrentData = string.Empty;
            IsComplete = false;
        }

        private void OnAlphanumericTyped(KeyData data)
        {
            CurrentData += data.Data;
            IsComplete = false;
        }

        private void OnSymbolTyped(KeyData data)
        {
            CurrentData += data.Data;
            IsComplete = false;
        }

        private void OnDeletionTyped(KeyData data)
        {
            CurrentData = string.Empty;
            IsComplete = false;
        }

        private void OnWordCompleted(KeyData data)
        {
            IsComplete = true;
        }
    }


    public enum KeypressTrigger
    {
        Alphanumeric,
        Symbol,
        Punctuation,
        Whitespace,
        Deletion
    }

    public enum WordState
    {
        NotStarted,
        Started,
        Interrupted,
        Completed
    }
}
