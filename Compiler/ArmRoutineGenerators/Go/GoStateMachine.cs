using System;
using FileIO;
using Compiler.LexicalAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum GoMachineState
    {
        START,
        GO,
        TO,
        DESTINATION
    }

    public class GoStateMachine : ISubStateMachine {
        private Dictionary<GoStateTransition, GoMachineState> transitions;
        public GoMachineState CurrentState { get; private set; }

        private GoCommand command;

        public GoStateMachine(FileManager fileManager) {
            CurrentState = GoMachineState.START;
            transitions = new Dictionary<GoStateTransition, GoMachineState>
            {
                { new GoStateTransition(GoMachineState.START, TokenType.END), GoMachineState.START },
                { new GoStateTransition(GoMachineState.START, TokenType.GO), GoMachineState.GO },
                { new GoStateTransition(GoMachineState.START, TokenType.GOTO), GoMachineState.TO },

                { new GoStateTransition(GoMachineState.GO, TokenType.TO), GoMachineState.TO },

                { new GoStateTransition(GoMachineState.TO, TokenType.INT), GoMachineState.DESTINATION },

                { new GoStateTransition(GoMachineState.DESTINATION, TokenType.END), GoMachineState.START },
            };

            this.command = new GoCommand(fileManager);
        }


        public GoMachineState GetNext(Token token)
        {
            GoMachineState nextState = this.CurrentState;
            GoStateTransition transition = new GoStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (nextState == GoMachineState.DESTINATION && token.Type != TokenType.END) {
                this.command.ConsumeToken(token);
            }

            Console.WriteLine("P: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            GoMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class GoStateTransition {

            public GoMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public GoStateTransition(GoMachineState currentState, TokenType token)
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
                GoStateTransition other = obj as GoStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}