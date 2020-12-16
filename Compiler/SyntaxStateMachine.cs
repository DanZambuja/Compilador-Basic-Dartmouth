using System;
using System.Collections.Generic;
using Compiler.LexicalAnalysis;
using Compiler.ArmRoutineGenerators;

namespace Compiler.SyntaxAnalysis
{
    public enum SyntaxMachineState
    {
        EMPTY,
        PRINT,
        LET,
        PRINT_MULTIPLE,
        FOR_LOOP,
        NEXT,
        VARIABLE_ASSIGN,
        DIM,
        READ,
        DATA
    }

    public delegate void OutputCompiledToWrite(string armCommand);

    public class SyntaxStateMachine 
    {
        public event OutputCompiledToWrite NotifyOutputCompiledToWrite;
        private Dictionary<SyntaxStateTransition, SyntaxMachineState> transitions;
        public SyntaxMachineState CurrentState { get; private set; }
        private int variableCounter = 0;
        private Dictionary<string, int> variableToIndex = new Dictionary<string, int>();

        private Dictionary<SyntaxMachineState, ICommand> commands;

        private ArmRoutineGenerator routines = new ArmRoutineGenerator();

        public SyntaxStateMachine() { 
            CurrentState = SyntaxMachineState.EMPTY;
            transitions = new Dictionary<SyntaxStateTransition, SyntaxMachineState>
            {
                { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.INT), SyntaxMachineState.EMPTY },
                { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.PRINT), SyntaxMachineState.PRINT },

                { new SyntaxStateTransition(SyntaxMachineState.PRINT, TokenType.STRING), SyntaxMachineState.PRINT_MULTIPLE },
                { new SyntaxStateTransition(SyntaxMachineState.PRINT, TokenType.INT), SyntaxMachineState.PRINT_MULTIPLE },

                { new SyntaxStateTransition(SyntaxMachineState.PRINT_MULTIPLE, TokenType.COMMA), SyntaxMachineState.PRINT },
                { new SyntaxStateTransition(SyntaxMachineState.PRINT_MULTIPLE, TokenType.INT), SyntaxMachineState.EMPTY },
                { new SyntaxStateTransition(SyntaxMachineState.PRINT_MULTIPLE, TokenType.END), SyntaxMachineState.EMPTY },
            };

            PrintCommand print = new PrintCommand();
            LetCommand let = new LetCommand();

            this.commands = new Dictionary<SyntaxMachineState, ICommand>{
                { SyntaxMachineState.PRINT, print },
                { SyntaxMachineState.PRINT_MULTIPLE, print },
                { SyntaxMachineState.LET, let }
            };
        }

        public void ConsumeIdentifiedTokenEvent(Token token) {
            if (token.Type == TokenType.ARRAY) {
                //Console.WriteLine(token.Text + " - " + token.IndexOrSize.ToString() + " - " + token.Type.ToString());
            } else {
                //Console.WriteLine(token.Text + " - " + token.Type.ToString());
            }
            this.MoveNext(token);
        }

        protected virtual void OnOutputCompiled(string armCommand) {
            NotifyOutputCompiledToWrite?.Invoke(armCommand);
        }

         public SyntaxMachineState GetNext(Token token)
        {
            SyntaxStateTransition transition = new SyntaxStateTransition(CurrentState, token.Type);
            SyntaxMachineState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + token.Text + " " + token.Type.ToString());
            Console.WriteLine(this.CurrentState + " -> " + nextState);
            return nextState;
        }

        public SyntaxMachineState MoveNext(Token token)
        {
            SyntaxMachineState nextState = GetNext(token);

            if (this.CurrentState != SyntaxMachineState.EMPTY && nextState != SyntaxMachineState.EMPTY) {
                this.commands[this.CurrentState].ConsumeToken(token);
            } else if (this.CurrentState != SyntaxMachineState.EMPTY && nextState == SyntaxMachineState.EMPTY) {
                this.commands[this.CurrentState].Clear();
            }
            
            this.CurrentState = nextState;
            return CurrentState;
        }

        public void FinalizeAnalysis() {
            this.routines.Finalize(this.variableCounter);
        }

    class SyntaxStateTransition {

            public SyntaxMachineState CurrentState { get; private set; }
            public TokenType Command { get; private set; }

            public SyntaxStateTransition(SyntaxMachineState currentState, TokenType command)
            {
                this.CurrentState = currentState;
                this.Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                SyntaxStateTransition other = obj as SyntaxStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }

        }
    }
}