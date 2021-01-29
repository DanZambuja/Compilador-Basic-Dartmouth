using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum LetMachineState
    {
        START,
        LET,
        VAR_FROM_INDEX,
        EQUALS,
        ARITHMETIC
    }

    public class LetStateMachine : ISubStateMachine {
        private Dictionary<LetStateTransition, LetMachineState> transitions;
        public LetMachineState CurrentState { get; private set; }
        private VariableTable variables;
        private LetCommand command;
        private ArithmeticStateMachine exp;
        private Token variable;

        public LetStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = LetMachineState.START;
            transitions = new Dictionary<LetStateTransition, LetMachineState>
            {
                { new LetStateTransition(LetMachineState.START, TokenType.END), LetMachineState.START },
                { new LetStateTransition(LetMachineState.START, TokenType.LET), LetMachineState.LET },

                { new LetStateTransition(LetMachineState.LET, TokenType.VAR), LetMachineState.VAR_FROM_INDEX},
                { new LetStateTransition(LetMachineState.LET, TokenType.ARRAY_ELEMENT), LetMachineState.VAR_FROM_INDEX},

                { new LetStateTransition(LetMachineState.VAR_FROM_INDEX, TokenType.EQUALS), LetMachineState.EQUALS },

                { new LetStateTransition(LetMachineState.EQUALS, TokenType.END), LetMachineState.START },
            };
            this.variables = variables;
            this.command = new LetCommand(fileManager);
            this.exp = new ArithmeticStateMachine(variables, fileManager);
        }


        public LetMachineState GetNext(Token token)
        {
            LetMachineState nextState = this.CurrentState;
            LetStateTransition transition = new LetStateTransition(CurrentState, token.Type);

            if (this.CurrentState == LetMachineState.LET && (token.Type == TokenType.VAR || token.Type == TokenType.ARRAY_ELEMENT)) {
                this.variable = token;
            }

            if (!(this.CurrentState == LetMachineState.EQUALS && token.Type != TokenType.END) && !transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (this.CurrentState == LetMachineState.EQUALS && token.Type == TokenType.END && nextState == LetMachineState.START) {
                this.exp.Reset();
                this.variables.variableToIndex[this.variable.Text] = this.variables.variableCounter++;
                this.command.GenerateStoreInMemInstructions(this.variables.variableToIndex[this.variable.Text]);
            } else if (this.CurrentState == LetMachineState.EQUALS) {
                this.exp.MoveToNextState(token);
            }

            Console.WriteLine("LET: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            LetMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class LetStateTransition {

            public LetMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public LetStateTransition(LetMachineState currentState, TokenType token)
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
                LetStateTransition other = obj as LetStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}