using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum IfMachineState
    {
        START,
        IF_FIRST_EXP,
        COMPARISON,
        SECOND_EXPRESSION,
        THEN,
        DESTINATION
    }

    public class IfStateMachine : ISubStateMachine {
        private Dictionary<IfStateTransition, IfMachineState> transitions;
        public IfMachineState CurrentState { get; private set; }
        private IfCommand command;
        private ArithmeticStateMachine exp;
        private Token comparison = null;
        private int destination = 0;

        public IfStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = IfMachineState.START;
            transitions = new Dictionary<IfStateTransition, IfMachineState>
            {
                { new IfStateTransition(IfMachineState.START, TokenType.END), IfMachineState.START },
                { new IfStateTransition(IfMachineState.START, TokenType.IF), IfMachineState.IF_FIRST_EXP },

                { new IfStateTransition(IfMachineState.IF_FIRST_EXP, TokenType.EQUALS), IfMachineState.COMPARISON },
                { new IfStateTransition(IfMachineState.IF_FIRST_EXP, TokenType.NOT_EQUAL), IfMachineState.COMPARISON },
                { new IfStateTransition(IfMachineState.IF_FIRST_EXP, TokenType.GREATER), IfMachineState.COMPARISON },
                { new IfStateTransition(IfMachineState.IF_FIRST_EXP, TokenType.GREATER_OR_EQUAL), IfMachineState.COMPARISON },
                { new IfStateTransition(IfMachineState.IF_FIRST_EXP, TokenType.LESS), IfMachineState.COMPARISON },
                { new IfStateTransition(IfMachineState.IF_FIRST_EXP, TokenType.LESS_OR_EQUAL), IfMachineState.COMPARISON },

                { new IfStateTransition(IfMachineState.COMPARISON, TokenType.THEN), IfMachineState.DESTINATION },

                { new IfStateTransition(IfMachineState.DESTINATION, TokenType.INT), IfMachineState.START }
            };

            this.command = new IfCommand(variables, fileManager);
            this.exp = new ArithmeticStateMachine(variables, fileManager);
        }


        public IfMachineState GetNext(Token token)
        {
            IfMachineState nextState = this.CurrentState;
            IfStateTransition transition = new IfStateTransition(CurrentState, token.Type);

            if ((this.CurrentState == IfMachineState.IF_FIRST_EXP && !this.IsComparator(token)) || 
                (this.CurrentState == IfMachineState.COMPARISON && token.Type != TokenType.THEN)) {
                this.exp.MoveToNextState(token);
            } else if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if ((nextState == IfMachineState.COMPARISON && this.IsComparator(token)) || 
                 (nextState == IfMachineState.DESTINATION && token.Type == TokenType.THEN)) {
                this.exp.Reset();

                if (nextState == IfMachineState.COMPARISON && this.IsComparator(token)) {
                    this.comparison = token;
                    this.command.MoveResultToSecondRegister();
                }

            } else if (nextState == IfMachineState.START && token.Type != TokenType.END) {
                this.destination = int.Parse(token.Text);
                this.command.IfInstructions(this.destination, this.comparison);
                this.comparison = null;
                this.destination = 0;
            }
            

            Console.WriteLine("IF: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        private bool IsComparator(Token token) {
            return token.Type switch {
                TokenType.EQUALS            => true,
                TokenType.NOT_EQUAL         => true,
                TokenType.GREATER           => true,
                TokenType.GREATER_OR_EQUAL  => true,
                TokenType.LESS              => true,
                TokenType.LESS_OR_EQUAL     => true,
                _                           => false
            };
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