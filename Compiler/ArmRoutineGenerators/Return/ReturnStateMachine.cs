using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum ReturnMachineState
    {
        START,
        RETURN
    }

    public class ReturnStateMachine : ISubStateMachine {
        private Dictionary<ReturnStateTransition, ReturnMachineState> transitions;
        public ReturnMachineState CurrentState { get; private set; }
        private ReturnCommand command;
        private VariableTable variables;

        public ReturnStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = ReturnMachineState.START;
            transitions = new Dictionary<ReturnStateTransition, ReturnMachineState>
            {
                { new ReturnStateTransition(ReturnMachineState.START, TokenType.END), ReturnMachineState.START },
                { new ReturnStateTransition(ReturnMachineState.START, TokenType.RETURN), ReturnMachineState.RETURN },

                { new ReturnStateTransition(ReturnMachineState.RETURN, TokenType.END), ReturnMachineState.START },
            };

            this.variables = variables;
            this.command = new ReturnCommand(variables, fileManager);
        }


        public ReturnMachineState GetNext(Token token)
        {
            ReturnMachineState nextState = this.CurrentState;
            ReturnStateTransition transition = new ReturnStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            Console.WriteLine("P: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            ReturnMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class ReturnStateTransition {

            public ReturnMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public ReturnStateTransition(ReturnMachineState currentState, TokenType token)
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
                ReturnStateTransition other = obj as ReturnStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}