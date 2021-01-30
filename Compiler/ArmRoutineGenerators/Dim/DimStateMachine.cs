using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum DimMachineState
    {
        START,
        DIM,
        ARRAY
    }

    public class DimStateMachine : ISubStateMachine {
        private Dictionary<DimStateTransition, DimMachineState> transitions;
        public DimMachineState CurrentState { get; private set; }
        private DimCommand command;
        private VariableTable variables;

        public DimStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = DimMachineState.START;
            transitions = new Dictionary<DimStateTransition, DimMachineState>
            {
                { new DimStateTransition(DimMachineState.START, TokenType.END), DimMachineState.START },
                { new DimStateTransition(DimMachineState.START, TokenType.DIM), DimMachineState.DIM },

                { new DimStateTransition(DimMachineState.DIM, TokenType.ARRAY), DimMachineState.ARRAY },

                { new DimStateTransition(DimMachineState.ARRAY, TokenType.END), DimMachineState.START }
            };
            this.variables = variables;
            this.command = new DimCommand(variables, fileManager);
        }


        public DimMachineState GetNext(Token token)
        {
            DimMachineState nextState = this.CurrentState;
            DimStateTransition transition = new DimStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("DIM: Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (this.CurrentState == DimMachineState.DIM && token.Type == TokenType.ARRAY) {
                this.command.ConsumeToken(token);
            }

            Console.WriteLine("DIM: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
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