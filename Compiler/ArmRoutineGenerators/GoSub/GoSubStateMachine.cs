using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum GoSubMachineState
    {
        START,
        GOSUB,
        LABEL
    }

    public class GoSubStateMachine : ISubStateMachine {
        private Dictionary<GoSubStateTransition, GoSubMachineState> transitions;
        public GoSubMachineState CurrentState { get; private set; }
        private GoSubCommand command;
        private VariableTable variables;
        
        public GoSubStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = GoSubMachineState.START;
            transitions = new Dictionary<GoSubStateTransition, GoSubMachineState>
            {
                { new GoSubStateTransition(GoSubMachineState.START, TokenType.END), GoSubMachineState.START },
                { new GoSubStateTransition(GoSubMachineState.START, TokenType.GOSUB), GoSubMachineState.GOSUB },
                
                { new GoSubStateTransition(GoSubMachineState.GOSUB, TokenType.INT), GoSubMachineState.LABEL },

                { new GoSubStateTransition(GoSubMachineState.LABEL, TokenType.END), GoSubMachineState.START }
            };

            this.variables = variables;
            this.command = new GoSubCommand(variables, fileManager);
        }


        public GoSubMachineState GetNext(Token token)
        {
            GoSubMachineState nextState = this.CurrentState;
            GoSubStateTransition transition = new GoSubStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("GOSUB: Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (this.CurrentState == GoSubMachineState.GOSUB && token.Type == TokenType.INT) {
                this.command.ConsumeToken(token);
            }

            Console.WriteLine("GOSUB: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            GoSubMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class GoSubStateTransition {

            public GoSubMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public GoSubStateTransition(GoSubMachineState currentState, TokenType token)
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
                GoSubStateTransition other = obj as GoSubStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}