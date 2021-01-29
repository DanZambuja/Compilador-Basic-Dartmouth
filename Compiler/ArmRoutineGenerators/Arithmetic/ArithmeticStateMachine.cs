using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum ArithmeticMachineState
    {
        START,
        EXP
    }

    public class ArithmeticStateMachine : ISubStateMachine {
        private Dictionary<ArithmeticStateTransition, ArithmeticMachineState> transitions;
        public ArithmeticMachineState CurrentState { get; private set; }
        private VariableTable variables;
        private ArithmeticCommand command;

        public ArithmeticStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = ArithmeticMachineState.START;
            transitions = new Dictionary<ArithmeticStateTransition, ArithmeticMachineState>
            {
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.END), ArithmeticMachineState.START },
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.INT), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.VAR), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.PLUS), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.MINUS), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.MULT), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.DIV), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.POWER), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.START, TokenType.OPENING_BRACES), ArithmeticMachineState.EXP },

                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.INT), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.VAR), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.PLUS), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.MINUS), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.MULT), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.DIV), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.POWER), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.OPENING_BRACES), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.CLOSING_BRACES), ArithmeticMachineState.EXP },
                { new ArithmeticStateTransition(ArithmeticMachineState.EXP, TokenType.END), ArithmeticMachineState.START },
            };

            this.variables = variables;
            this.command = new ArithmeticCommand(variables, fileManager);
        }

        public void Reset() {
            this.CurrentState = ArithmeticMachineState.START;
            this.command.EndOfExpression();
        }


        public ArithmeticMachineState GetNext(Token token)
        {
            ArithmeticMachineState nextState = this.CurrentState;
            ArithmeticStateTransition transition = new ArithmeticStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (token.Type != TokenType.END) {
                this.command.ConsumeToken(token);
            } else {
                this.command.EndOfExpression();
            }

            Console.WriteLine("Arithmetic: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            ArithmeticMachineState nextState = GetNext(token);
            this.CurrentState = nextState;
        }

        class ArithmeticStateTransition {

            public ArithmeticMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public ArithmeticStateTransition(ArithmeticMachineState currentState, TokenType token)
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
                ArithmeticStateTransition other = obj as ArithmeticStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}