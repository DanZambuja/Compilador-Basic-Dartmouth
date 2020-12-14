using System;
using System.Collections.Generic;

namespace Compiler.SyntaxAnalysis
{
    public enum SyntaxMachineState
    {
        EMPTY,
        PRINT,
        FOR_LOOP,
        VARIABLE_ASSIGN
    }

    public enum TokenType {
        SEQUENCE_ID,
        IDENTIFIER,
        RESERVED,
        OPENING_BRACES,
        CLOSING_BRACES,
        SPECIAL
    }

    public class Token {

        public TokenType Type { get; private set; }
        public string Text { get; private set; }

        public Token(TokenType type, string token) {
            this.Text = token;
            this.Type = type;
        }
    }

    public delegate void OutputCompiledToWrite(string armCommand);

    public class SyntaxStateMachine 
    {
        public event OutputCompiledToWrite NotifyOutputCompiledToWrite;
        private Dictionary<SyntaxStateTransition, SyntaxMachineState> transitions;
        public SyntaxMachineState CurrentState { get; private set; }
        private int variableCounter = 0;
        private Dictionary<string, int> variableToIndex = new Dictionary<string, int>();

        public SyntaxStateMachine() { 
            CurrentState = SyntaxMachineState.EMPTY;
            transitions = new Dictionary<SyntaxStateTransition, SyntaxMachineState>
            {
                { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.SEQUENCE_ID), SyntaxMachineState.IN_SEQUENCE },
            };
        }

        public void ConsumeIdentifiedTokenEvent(string token) {
            Console.WriteLine("-------");
            Console.WriteLine(token);
        }

        protected virtual void OnOutputCompiled(string armCommand) {
            NotifyOutputCompiledToWrite?.Invoke(armCommand);
        }

         public SyntaxMachineState GetNext(Token token)
        {
            SyntaxStateTransition transition = new SyntaxStateTransition(CurrentState, token.Type);
            SyntaxMachineState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + token.Text + " " + token.Type.ToString());
            return nextState;
        }

        public SyntaxMachineState MoveNext(Token token)
        {
            SyntaxMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
            return CurrentState;
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
}