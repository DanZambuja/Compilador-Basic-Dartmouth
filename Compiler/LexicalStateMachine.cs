using System;
using Compiler.AsciiCategorizer;
using System.Collections.Generic;

namespace Compiler.LexicalAnalysis
{
    public enum LexicalMachineState
    {
        EMPTY,
        INT_TOKEN,
        STRING_TOKEN,
        SPECIAL_TOKEN
    }
    
    public delegate void TokenIdentified(string token);

    public class LexicalStateMachine {
        private Categorizer categorizer = new Categorizer();
        private Dictionary<LexicalStateTransition, LexicalMachineState> transitions;
        public event TokenIdentified NotifyTokenIdentified;
        private string currentToken = string.Empty;
        public LexicalMachineState CurrentState { get; private set; }

        public LexicalStateMachine() {
            CurrentState = LexicalMachineState.EMPTY;
            transitions = new Dictionary<LexicalStateTransition, LexicalMachineState>
            {
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.DIGIT), LexicalMachineState.INT_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.LETTER), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.DELIMITER), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.CONTROL), LexicalMachineState.EMPTY },

                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.DIGIT), LexicalMachineState.INT_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.DELIMITER), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.CONTROL), LexicalMachineState.EMPTY },

                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.LETTER), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.DIGIT), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.OPENING_PAR), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.CLOSING_PAR), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN},
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.DELIMITER), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.CONTROL), LexicalMachineState.EMPTY },

                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.LETTER), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.DIGIT), LexicalMachineState.INT_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.DELIMITER), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.CONTROL), LexicalMachineState.EMPTY },
            };
        }

        protected virtual void OnTokenIdentified() {
            NotifyTokenIdentified?.Invoke(this.currentToken);
        }

        public void ConsumeCategorizedSymbolEvent(AsciiAtom symbol) {
            this.MoveNext(symbol);
        }

        private void UpdateTokenState(AsciiAtom command) {
            if (command.Category != AtomType.CONTROL && command.Category != AtomType.DELIMITER)
                this.currentToken = this.currentToken + command.Symbol;
        }

        private void ClearToken() {
            this.currentToken = string.Empty;
        }

        public LexicalMachineState GetNext(AtomType command)
        {
            LexicalStateTransition transition = new LexicalStateTransition(CurrentState, command);
            LexicalMachineState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
            return nextState;
        }

        public LexicalMachineState MoveNext(AsciiAtom command)
        {
            LexicalMachineState nextState = GetNext(command.Category);

            if (nextState == LexicalMachineState.EMPTY ||
                nextState == LexicalMachineState.SPECIAL_TOKEN && 
                (this.CurrentState == LexicalMachineState.STRING_TOKEN || this.CurrentState == LexicalMachineState.INT_TOKEN) ||
                (nextState == LexicalMachineState.INT_TOKEN || nextState == LexicalMachineState.STRING_TOKEN) && 
                this.CurrentState == LexicalMachineState.SPECIAL_TOKEN) {
                
                this.OnTokenIdentified();
                this.ClearToken();
            }

            this.UpdateTokenState(command);
            this.CurrentState = nextState;
            return CurrentState;
        }

        class LexicalStateTransition {

            public LexicalMachineState CurrentState { get; private set; }
            public AtomType Command { get; private set; }

            public LexicalStateTransition(LexicalMachineState currentState, AtomType command)
            {
                this.CurrentState = currentState;
                this.Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                LexicalStateTransition other = obj as LexicalStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }

        }
    }
}