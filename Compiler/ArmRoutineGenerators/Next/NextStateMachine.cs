using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum NextMachineState
    {
        START,
        NEXT,
        VAR
    }

    public class NextStateMachine : ISubStateMachine {
        private Dictionary<NextStateTransition, NextMachineState> transitions;
        public NextMachineState CurrentState { get; private set; }
        private NextCommand command;
        private VariableTable variables;

        public NextStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = NextMachineState.START;
            transitions = new Dictionary<NextStateTransition, NextMachineState>
            {
                { new NextStateTransition(NextMachineState.START, TokenType.END), NextMachineState.START },
                { new NextStateTransition(NextMachineState.START, TokenType.NEXT), NextMachineState.NEXT },

                { new NextStateTransition(NextMachineState.NEXT, TokenType.VAR), NextMachineState.VAR },
                { new NextStateTransition(NextMachineState.NEXT, TokenType.ARRAY), NextMachineState.VAR },

                { new NextStateTransition(NextMachineState.VAR, TokenType.END), NextMachineState.START },
            };
            this.variables = variables;
            this.command = new NextCommand(variables, fileManager);
        }


        public NextMachineState GetNext(Token token)
        {
            NextMachineState nextState = this.CurrentState;
            NextStateTransition transition = new NextStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("NEXT: Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (nextState == NextMachineState.VAR)
                this.command.AddStepToLoopVariableAndLoopBack(token);

            Console.WriteLine("NEXT: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            NextMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class NextStateTransition {

            public NextMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public NextStateTransition(NextMachineState currentState, TokenType token)
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
                NextStateTransition other = obj as NextStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}