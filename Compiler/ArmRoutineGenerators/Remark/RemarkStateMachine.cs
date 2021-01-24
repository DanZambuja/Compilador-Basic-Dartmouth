using System;
using FileIO;
using Compiler.LexicalAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum RemarkMachineState
    {
        START,
        REMARK
    }

    public class RemarkStateMachine : ISubStateMachine {
        private Dictionary<RemarkStateTransition, RemarkMachineState> transitions;
        public RemarkMachineState CurrentState { get; private set; }
        private RemarkCommand command;

        public RemarkStateMachine(FileManager fileManager) {
            CurrentState = RemarkMachineState.START;
            transitions = new Dictionary<RemarkStateTransition, RemarkMachineState>
            {
                { new RemarkStateTransition(RemarkMachineState.START, TokenType.END), RemarkMachineState.START },
                { new RemarkStateTransition(RemarkMachineState.START, TokenType.REMARK), RemarkMachineState.REMARK },

                { new RemarkStateTransition(RemarkMachineState.REMARK, TokenType.END), RemarkMachineState.START },
            };

            this.command = new RemarkCommand(fileManager);
        }


        public RemarkMachineState GetNext(Token token)
        {
            RemarkMachineState nextState = this.CurrentState;
            RemarkStateTransition transition = new RemarkStateTransition(CurrentState, token.Type);

            if ((this.CurrentState == RemarkMachineState.REMARK && token.Type == TokenType.END || this.CurrentState == RemarkMachineState.START) && !transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            Console.WriteLine("P: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            RemarkMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class RemarkStateTransition {

            public RemarkMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public RemarkStateTransition(RemarkMachineState currentState, TokenType token)
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
                RemarkStateTransition other = obj as RemarkStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}