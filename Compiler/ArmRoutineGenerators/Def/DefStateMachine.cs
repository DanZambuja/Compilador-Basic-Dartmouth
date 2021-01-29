using System;
using FileIO;
using Compiler.LexicalAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum DefMachineState
    {
        START
    }

    public class DefStateMachine : ISubStateMachine {
        private Dictionary<DefStateTransition, DefMachineState> transitions;
        public DefMachineState CurrentState { get; private set; }
        private DefCommand command;

        public DefStateMachine(FileManager fileManager) {
            CurrentState = DefMachineState.START;
            transitions = new Dictionary<DefStateTransition, DefMachineState>
            {
                { new DefStateTransition(DefMachineState.START, TokenType.END), DefMachineState.START }
            };

            this.command = new DefCommand(fileManager);
        }


        public DefMachineState GetNext(Token token)
        {
            DefMachineState nextState = this.CurrentState;
            DefStateTransition transition = new DefStateTransition(CurrentState, token.Type);

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
            DefMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class DefStateTransition {

            public DefMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public DefStateTransition(DefMachineState currentState, TokenType token)
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
                DefStateTransition other = obj as DefStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}