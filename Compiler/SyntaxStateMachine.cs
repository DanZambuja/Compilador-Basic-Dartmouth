using System;
using System.Collections.Generic;

namespace Compiler.SyntaxAnalysis
{
    public enum SyntaxMachineState
    {
        EMPTY,
    }

    public enum TokenType {
        IDENTIFIER,
        RESERVED,
        OPENING_BRACES,
        CLOSING_BRACES,
        SPECIAL
    }

    public class Token {

        public TokenType Category {
            get {
                return TokenType.IDENTIFIER;
            }
        }
        public string Text { get; private set; }

        public Token(string token) {
            this.Text = token;
        }
    }

    public class SyntaxStateMachine 
    {
        private Dictionary<SyntaxStateTransition, SyntaxMachineState> transitions;
        public SyntaxMachineState CurrentState { get; private set; }

        public SyntaxStateMachine() { 
            CurrentState = SyntaxMachineState.EMPTY;
            transitions = new Dictionary<SyntaxStateTransition, SyntaxMachineState>
            {
                { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.IDENTIFIER), SyntaxMachineState.EMPTY },
            };
        }

        public void ConsumeIdentifiedTokenEvent(string token) {
            Console.WriteLine("-------");
            Console.WriteLine(token);
        }
    }

    class SyntaxStateTransition {

            public SyntaxMachineState CurrentState { get; private set; }
            public TokenType Command { get; private set; }

            public SyntaxStateTransition(SyntaxMachineState currentState, TokenType command)
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
                SyntaxStateTransition other = obj as SyntaxStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }

        }
}