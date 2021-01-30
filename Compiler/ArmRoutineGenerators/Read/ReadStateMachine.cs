using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum ReadMachineState
    {
        START,
        READ,
        READ_ELEMENT,
        COMMA
    }

    public class ReadStateMachine : ISubStateMachine {
        private Dictionary<ReadStateTransition, ReadMachineState> transitions;
        public ReadMachineState CurrentState { get; private set; }
        private ReadCommand command;
        private VariableTable variables;

        public ReadStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = ReadMachineState.START;
            transitions = new Dictionary<ReadStateTransition, ReadMachineState>
            {
                { new ReadStateTransition(ReadMachineState.START,        TokenType.READ),   ReadMachineState.READ },

                { new ReadStateTransition(ReadMachineState.READ,         TokenType.VAR),    ReadMachineState.READ_ELEMENT },
                { new ReadStateTransition(ReadMachineState.READ,         TokenType.ARRAY),  ReadMachineState.READ_ELEMENT },

                { new ReadStateTransition(ReadMachineState.READ_ELEMENT, TokenType.COMMA),  ReadMachineState.READ_ELEMENT },
                { new ReadStateTransition(ReadMachineState.READ_ELEMENT, TokenType.END),    ReadMachineState.START },

                { new ReadStateTransition(ReadMachineState.COMMA,        TokenType.VAR),    ReadMachineState.READ_ELEMENT },
                { new ReadStateTransition(ReadMachineState.COMMA,        TokenType.ARRAY),  ReadMachineState.READ_ELEMENT },
                { new ReadStateTransition(ReadMachineState.COMMA,        TokenType.END),    ReadMachineState.START }
            };

            this.variables = variables;
            this.command = new ReadCommand(variables, fileManager);
        }


        public ReadMachineState GetNext(Token token)
        {
            ReadMachineState nextState = this.CurrentState;
            ReadStateTransition transition = new ReadStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("READ: Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (this.CurrentState == ReadMachineState.START) {
                this.variables.ResetReadVariables();
            } else if ((this.CurrentState ==  ReadMachineState.READ || this.CurrentState == ReadMachineState.COMMA) && 
                (token.Type == TokenType.VAR || token.Type == TokenType.ARRAY)) {
                this.command.ConsumeToken(token);
            }

            Console.WriteLine("READ: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            ReadMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class ReadStateTransition {

            public ReadMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public ReadStateTransition(ReadMachineState currentState, TokenType token)
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
                ReadStateTransition other = obj as ReadStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}