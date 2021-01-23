using System;
using System.Collections.Generic;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.ArmRoutineGenerators;

namespace Compiler.SyntaxAnalysis
{
    public enum SyntaxMachineState
    {
        START,
        PRINT,
        LET,
        FOR,
        DIM,
        READ,
        REMARK
    }

    public delegate void OutputCompiledToWrite(string armCommand);

    public class SyntaxStateMachine 
    {
        public event OutputCompiledToWrite NotifyOutputCompiledToWrite;
        private Dictionary<SyntaxStateTransition, SyntaxMachineState> transitions;
        public SyntaxMachineState CurrentState { get; private set; }
        private int variableCounter = 0;
        private Dictionary<string, int> variableToIndex = new Dictionary<string, int>();
        private SyntaxEngine engine = new SyntaxEngine();
        private SequenceIdLabelCommand sequenceId = new SequenceIdLabelCommand();

        private FileManager fileManager;

        public SyntaxStateMachine(FileManager fileManager) { 
            CurrentState = SyntaxMachineState.EMPTY;
            transitions = new Dictionary<SyntaxStateTransition, SyntaxMachineState> 
            {
                { new SyntaxStateTransition(SyntaxMachineState.START, TokenType.INT), SyntaxMachineState.START }

                { new SyntaxStateTransition(SyntaxMachineState.START, TokenType.PRINT), SyntaxMachineState.PRINT }
                { new SyntaxStateTransition(SyntaxMachineState.START, TokenType.LET), SyntaxMachineState.LET }
                { new SyntaxStateTransition(SyntaxMachineState.START, TokenType.FOR), SyntaxMachineState.FOR }
                { new SyntaxStateTransition(SyntaxMachineState.START, TokenType.DIM), SyntaxMachineState.DIM }
                { new SyntaxStateTransition(SyntaxMachineState.START, TokenType.REMARK), SyntaxMachineState.REMARK }
                { new SyntaxStateTransition(SyntaxMachineState.START, TokenType.GO), SyntaxMachineState.GO }

                { new LexicalStateTransition(SyntaxMachineState.PRINT, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.LET, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.FOR, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.DIM, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.REMARK, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.GO, TokenType.END), PrintMachineState.START },
            }

            this.fileManager = fileManager;
        }

        public void ConsumeIdentifiedTokenEvent(Token token) {
            if (token.Type == TokenType.ARRAY || token.Type == TokenType.ARRAY_ELEMENT) {
                Console.WriteLine("S1: " + token.Text + " - " + token.IndexOrSize.ToString() + " - " + token.Type.ToString());
            } else {
                Console.WriteLine("S2: " + token.Text + " - " + token.Type.ToString());
            }
            this.MoveNext(token);
        }

        protected virtual void OnOutputCompiled(string armCommand) {
            NotifyOutputCompiledToWrite?.Invoke(armCommand);
        }

        public SyntaxMachineState GetNext(Token token)
        {
            SyntaxMachineState nextState;
            SyntaxStateTransition transition = new SyntaxStateTransition(CurrentState, token.Type);

            if ((this.CurrentState == SyntaxMachineState.START || this.CurrentState != SyntaxMachineState.START && token.Type == TokenType.END) && 
                !transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + token.Text + " " + token.Type.ToString());

            if (this.CurrentState == SyntaxMachineState.START && token.Type == TokenType.INT && nextState == SyntaxMachineState.START) {
                this.sequenceId.ConsumeToken(token)
            }
            
            if (this.CurrentState != SyntaxMachineState.START && token.Type != TokenType.END) {
                this.engine.
            }
            
            Console.WriteLine("S: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public SyntaxMachineState MoveNext(Token token)
        {
            SyntaxMachineState nextState = GetNext(token);
            
            this.CurrentState = nextState;
            return CurrentState;
        }

        public void End() {
            FinalizationCommand final = new FinalizationCommand(this.fileManager);
            final.AllocateMemorySpaceForVariables(this.variableCounter);
        }

        class SyntaxStateTransition {

            public SyntaxMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public SyntaxStateTransition(SyntaxMachineState currentState, TokenType token)
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
                SyntaxStateTransition other = obj as SyntaxStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }

        }

        class SyntaxEngine {
            private Dictionary<SyntaxMachineState, ISubStateMachine> currentSubStateMachine = new Dictionary<SyntaxMachineState, ISubStateMachine> 
            {
                { SyntaxMachineState.PRINT, new PrintStateMachine()}
            }

            public void ConsumeToken(SyntaxMachineState currentState, Token token) {

            }
        }
    }
}