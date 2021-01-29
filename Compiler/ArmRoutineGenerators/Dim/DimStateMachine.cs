using System;
using FileIO;
using Compiler.LexicalAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum DimMachineState
    {
        START
    }

    public class DimStateMachine : ISubStateMachine {
        private Dictionary<DimStateTransition, DimMachineState> transitions;
        public DimMachineState CurrentState { get; private set; }
        private DimCommand command;

        public DimStateMachine(FileManager fileManager) {
            CurrentState = DimMachineState.START;
            transitions = new Dictionary<DimStateTransition, DimMachineState>
            {
                { new DimStateTransition(DimMachineState.START, TokenType.END), DimMachineState.START }
            };

            this.command = new DimCommand(fileManager);
        }


        public DimMachineState GetNext(Token token)
        {
            DimMachineState nextState = this.CurrentState;
            DimStateTransition transition = new DimStateTransition(CurrentState, token.Type);

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
            DimMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class DimStateTransition {

            public DimMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public DimStateTransition(DimMachineState currentState, TokenType token)
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
                DimStateTransition other = obj as DimStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}