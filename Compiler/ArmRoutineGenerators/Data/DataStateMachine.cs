using System;
using FileIO;
using Compiler.LexicalAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum DataMachineState
    {
        START
    }

    public class DataStateMachine : ISubStateMachine {
        private Dictionary<DataStateTransition, DataMachineState> transitions;
        public DataMachineState CurrentState { get; private set; }
        private DataCommand command;

        public DataStateMachine(FileManager fileManager) {
            CurrentState = DataMachineState.START;
            transitions = new Dictionary<DataStateTransition, DataMachineState>
            {
                { new DataStateTransition(DataMachineState.START, TokenType.END), DataMachineState.START }
            };

            this.command = new DataCommand(fileManager);
        }


        public DataMachineState GetNext(Token token)
        {
            DataMachineState nextState = this.CurrentState;
            DataStateTransition transition = new DataStateTransition(CurrentState, token.Type);

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
            DataMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class DataStateTransition {

            public DataMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public DataStateTransition(DataMachineState currentState, TokenType token)
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
                DataStateTransition other = obj as DataStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}