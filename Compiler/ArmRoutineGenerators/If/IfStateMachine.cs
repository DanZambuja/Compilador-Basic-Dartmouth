using System;
using FileIO;
using Compiler.LexicalAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum IfMachineState
    {
        START
    }

    public class IfStateMachine : ISubStateMachine {
        private Dictionary<IfStateTransition, IfMachineState> transitions;
        public IfMachineState CurrentState { get; private set; }
        private IfCommand command;

        public IfStateMachine(FileManager fileManager) {
            CurrentState = IfMachineState.START;
            transitions = new Dictionary<IfStateTransition, IfMachineState>
            {
                { new IfStateTransition(IfMachineState.START, TokenType.END), IfMachineState.START }
            };

            this.command = new IfCommand(fileManager);
        }


        public IfMachineState GetNext(Token token)
        {
            IfMachineState nextState = this.CurrentState;
            IfStateTransition transition = new IfStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (token.Type != TokenType.END) {
                this.command.ConsumeToken(token);
            }

            Console.WriteLine("IF: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            IfMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class IfStateTransition {

            public IfMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public IfStateTransition(IfMachineState currentState, TokenType token)
            {
                this.CurrentState = currentState;
                this.Token = token;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Token.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                IfStateTransition other = obj as IfStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}