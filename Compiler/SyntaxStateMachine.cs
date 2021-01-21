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
        SEQ_ID,
        PRINT,
        LET,
        FOR,
        DIM,
        READ,
        REMARK
        
        // EMPTY,
        // PRINT,
        // PRINT_MULTIPLE,
        // LET,
        // LET_VAR,
        // LET_EQ,
        // LET_EXP,
        // LET_OP,
        // FOR_LOOP,
        // NEXT,
        // VARIABLE_ASSIGN,
        // DIM,
        // READ,
        // DATA,
        // SEQ_ID,
        // GO,
        // REMARK
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
        private FileManager fileManager;
        private LetCommand let;
        private LetFirstArithmeticCommand letFirst;
        private LetSecondArithmeticCommand letSecond;

        public SyntaxStateMachine(FileManager fileManager) { 
            CurrentState = SyntaxMachineState.EMPTY;
            transitions = new Dictionary<SyntaxStateTransition, SyntaxMachineState> 
            {
                { new SyntaxStateTransition(SyntaxMachineState.START, TokenType.INT), SyntaxMachineState.SEQ_ID }

                { new SyntaxStateTransition(SyntaxMachineState.SEQ_ID, TokenType.PRINT), SyntaxMachineState.PRINT}
                { new SyntaxStateTransition(SyntaxMachineState.SEQ_ID, TokenType.LET), SyntaxMachineState.LET}
                { new SyntaxStateTransition(SyntaxMachineState.SEQ_ID, TokenType.FOR), SyntaxMachineState.FOR}
                { new SyntaxStateTransition(SyntaxMachineState.SEQ_ID, TokenType.DIM), SyntaxMachineState.DIM}
                { new SyntaxStateTransition(SyntaxMachineState.SEQ_ID, TokenType.REMARK), SyntaxMachineState.REMARK}

                { new LexicalStateTransition(SyntaxMachineState.PRINT, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.LET, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.FOR, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.DIM, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.REMARK, TokenType.END), PrintMachineState.START },
                { new LexicalStateTransition(SyntaxMachineState.GO, TokenType.END), PrintMachineState.START },
            }
            // transitions = new Dictionary<SyntaxStateTransition, SyntaxMachineState>
            // {
            //     { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.INT), SyntaxMachineState.EMPTY },
            //     { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.PRINT), SyntaxMachineState.PRINT },
            //     { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.LET), SyntaxMachineState.LET },
            //     { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.GOTO), SyntaxMachineState.GO },
            //     { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.GO), SyntaxMachineState.GO },
            //     { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.END), SyntaxMachineState.EMPTY },
            //     { new SyntaxStateTransition(SyntaxMachineState.EMPTY, TokenType.REMARK), SyntaxMachineState.REMARK },

            //     { new SyntaxStateTransition(SyntaxMachineState.REMARK, TokenType.END), SyntaxMachineState.EMPTY },
                
            //     { new SyntaxStateTransition(SyntaxMachineState.GO, TokenType.TO), SyntaxMachineState.GO },
            //     { new SyntaxStateTransition(SyntaxMachineState.GO, TokenType.INT), SyntaxMachineState.EMPTY },

            //     { new SyntaxStateTransition(SyntaxMachineState.LET, TokenType.VAR), SyntaxMachineState.LET_VAR },

            //     { new SyntaxStateTransition(SyntaxMachineState.LET_VAR, TokenType.EQUALS), SyntaxMachineState.LET_EQ },

            //     { new SyntaxStateTransition(SyntaxMachineState.LET_EQ, TokenType.INT), SyntaxMachineState.LET_EXP },

            //     { new SyntaxStateTransition(SyntaxMachineState.LET_EXP, TokenType.PLUS), SyntaxMachineState.LET_OP },
            //     { new SyntaxStateTransition(SyntaxMachineState.LET_EXP, TokenType.MINUS), SyntaxMachineState.LET_OP },
            //     { new SyntaxStateTransition(SyntaxMachineState.LET_EXP, TokenType.MULT), SyntaxMachineState.LET_OP },
            //     { new SyntaxStateTransition(SyntaxMachineState.LET_EXP, TokenType.DIV), SyntaxMachineState.LET_OP },
            //     { new SyntaxStateTransition(SyntaxMachineState.LET_EXP, TokenType.END), SyntaxMachineState.EMPTY },

            //     { new SyntaxStateTransition(SyntaxMachineState.LET_OP, TokenType.INT), SyntaxMachineState.LET_EXP },

            //     { new SyntaxStateTransition(SyntaxMachineState.PRINT, TokenType.STRING), SyntaxMachineState.PRINT_MULTIPLE },
            //     { new SyntaxStateTransition(SyntaxMachineState.PRINT, TokenType.INT), SyntaxMachineState.PRINT_MULTIPLE },
            //     { new SyntaxStateTransition(SyntaxMachineState.PRINT, TokenType.VAR), SyntaxMachineState.PRINT_MULTIPLE },
            //     { new SyntaxStateTransition(SyntaxMachineState.PRINT, TokenType.END), SyntaxMachineState.EMPTY },

            //     { new SyntaxStateTransition(SyntaxMachineState.PRINT_MULTIPLE, TokenType.COMMA), SyntaxMachineState.PRINT },
            //     { new SyntaxStateTransition(SyntaxMachineState.PRINT_MULTIPLE, TokenType.INT), SyntaxMachineState.EMPTY },
            //     { new SyntaxStateTransition(SyntaxMachineState.PRINT_MULTIPLE, TokenType.END), SyntaxMachineState.EMPTY },
            // };

            this.fileManager = fileManager;
            this.let = new LetCommand(fileManager);
            this.letFirst = new LetFirstArithmeticCommand(fileManager);
            this.letSecond = new LetSecondArithmeticCommand(fileManager);

            PrintCommand print = new PrintCommand(fileManager);
            SequenceIdLabelCommand sequenceIdLabel = new SequenceIdLabelCommand(fileManager);
            GoCommand go = new GoCommand(fileManager);

            this.commands = new Dictionary<SyntaxMachineState, ICommand>{
                { SyntaxMachineState.PRINT, print },
                { SyntaxMachineState.PRINT_MULTIPLE, print },
                { SyntaxMachineState.SEQ_ID, sequenceIdLabel },
                { SyntaxMachineState.GO, go }
            };
        }

        public void ConsumeIdentifiedTokenEvent(Token token) {
            if (token.Type == TokenType.ARRAY) {
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
            SyntaxStateTransition transition = new SyntaxStateTransition(CurrentState, token.Type);
            SyntaxMachineState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + token.Text + " " + token.Type.ToString());
            Console.WriteLine("S: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            return nextState;
        }

        public SyntaxMachineState MoveNext(Token token)
        {
            if (this.CurrentState == SyntaxMachineState.REMARK && token.Type != TokenType.END)
                return this.CurrentState;

            SyntaxMachineState nextState = GetNext(token);
            if (this.CurrentState == SyntaxMachineState.LET && nextState == SyntaxMachineState.LET_VAR) {
                this.variableToIndex[token.Text] = this.variableCounter;
                this.let.ReceiveVariableIndex(this.variableCounter);
                this.variableCounter++;
            } else if (this.CurrentState == SyntaxMachineState.LET_EXP && nextState == SyntaxMachineState.EMPTY) {
                this.let.GenerateStoreInMemInstructions();
            } else if (this.CurrentState == SyntaxMachineState.LET_EQ && nextState == SyntaxMachineState.LET_EXP) {
                this.letFirst.ReceivedLeftSideOfOperation(token);
            } else if (this.CurrentState == SyntaxMachineState.LET_EXP && nextState == SyntaxMachineState.LET_OP) {
                this.letSecond.ReceiveOperation(token);
            } else if (this.CurrentState == SyntaxMachineState.LET_OP && nextState == SyntaxMachineState.LET_EXP) {
                this.letSecond.ReceiveRightSideOfOperation(token);
            }
            else if (this.CurrentState != SyntaxMachineState.EMPTY && nextState != SyntaxMachineState.EMPTY &&
                    !(this.CurrentState == SyntaxMachineState.LET_VAR && nextState == SyntaxMachineState.LET_EQ)) {
                this.commands[this.CurrentState].ConsumeToken(token);
            } else if (this.CurrentState == SyntaxMachineState.GO && 
                       nextState == SyntaxMachineState.EMPTY && 
                       token.Type == TokenType.INT) {
                this.commands[SyntaxMachineState.GO].ConsumeToken(token);
            } 
            else if (this.CurrentState != SyntaxMachineState.EMPTY && 
                       nextState == SyntaxMachineState.EMPTY &&
                       token.Type == TokenType.INT) {
                this.commands[SyntaxMachineState.SEQ_ID].ConsumeToken(token);
            } else if (this.CurrentState == SyntaxMachineState.EMPTY && 
                       nextState == SyntaxMachineState.EMPTY &&
                       token.Type == TokenType.INT) {
                this.commands[SyntaxMachineState.SEQ_ID].ConsumeToken(token);
            }
            
            this.CurrentState = nextState;
            return CurrentState;
        }

        public void End() {
            FinalizationCommand final = new FinalizationCommand(this.fileManager);
            final.AllocateMemorySpaceForVariables(this.variableCounter);
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