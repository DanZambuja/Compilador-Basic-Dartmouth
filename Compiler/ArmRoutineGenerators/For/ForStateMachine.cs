using System;
using FileIO;
using Compiler.LexicalAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum ForMachineState
    {
        START
    }

    public class ForStateMachine : ISubStateMachine {
        private Dictionary<ForStateTransition, ForMachineState> transitions;
        public ForMachineState CurrentState { get; private set; }
        private ForCommand command;

        public ForStateMachine(FileManager fileManager) {
            CurrentState = ForMachineState.START;
            transitions = new Dictionary<ForStateTransition, ForMachineState>
            {
                { new ForStateTransition(ForMachineState.START, TokenType.END), ForMachineState.START }
            };

            this.command = new ForCommand(fileManager);
        }


        public ForMachineState GetNext(Token token)
        {
            ForMachineState nextState = this.CurrentState;
            ForStateTransition transition = new ForStateTransition(CurrentState, token.Type);

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
            ForMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class ForStateTransition {

            public ForMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public ForStateTransition(ForMachineState currentState, TokenType token)
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
                ForStateTransition other = obj as ForStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}