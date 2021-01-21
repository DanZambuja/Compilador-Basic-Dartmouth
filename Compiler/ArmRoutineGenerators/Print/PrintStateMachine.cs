using System;
using Compiler.AsciiCategorizer;
using System.Collections.Generic;

namespace Compiler.LexicalAnalysis
{
    public enum PrintMachineState
    {
        START,
        PRINT,
        PRINT_MULTIPLE
    }
    
    public delegate void TokenIdentified(Token token);

    public class PrintStateMachine {
        private Categorizer categorizer = new Categorizer();
        private Dictionary<PrintStateTransition, PrintMachineState> transitions;
        public event TokenIdentified NotifyTokenIdentified;
        private string currentToken = string.Empty;
        private int indexOrSize = 0;
        public PrintMachineState CurrentState { get; private set; }

        public LexicalStateMachine() {
            CurrentState = PrintMachineState.START;
            transitions = new Dictionary<LexicalStateTransition, LexicalMachineState>
            {
                { new LexicalStateTransition(PrintMachineState.START, TokenType.PRINT), PrintMachineState.PRINT },
                
                { new LexicalStateTransition(PrintMachineState.PRINT, TokenType.STRING), PrintMachineState.PRINT_MULTIPLE },
                { new LexicalStateTransition(PrintMachineState.PRINT, TokenType.VAR), PrintMachineState.PRINT_MULTIPLE },
                
            };
        }

        public void ConsumeCategorizedSymbolEvent(AsciiAtom symbol) {
            this.MoveNext(symbol);
        }

        private void UpdateTokenState(AsciiAtom command) {
            if (command.Category != AtomType.CONTROL && 
                command.Category != AtomType.DELIMITER &&
                command.Category != AtomType.OPENING_PAR &&
                command.Category != AtomType.CLOSING_PAR)
                if (this.CurrentState == LexicalMachineState.ARRAY_TOKEN) {
                    this.indexOrSize = Int32.Parse(command.Symbol.ToString());
                } 
                else {
                    this.currentToken = this.currentToken + command.Symbol;
                }
        }

        private void ClearToken() {
            this.currentToken = string.Empty;
        }

        public PrintMachineState GetNext(AtomType command)
        {
            LexicalStateTransition transition = new LexicalStateTransition(CurrentState, command);
            LexicalMachineState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
            Console.WriteLine("L :" + this.CurrentState + " -> " + nextState + " : " + command.ToString());
            return nextState;
        }

        public PrintMachineState MoveNext(AsciiAtom command)
        {
            PrintMachineState nextState = GetNext(command.Category);

            if (nextState == LexicalMachineState.VAR_TOKEN && this.CurrentState == LexicalMachineState.EMPTY) {
                Console.WriteLine("1 Curr token: " + this.currentToken);
                this.UpdateTokenState(command);
                Console.WriteLine("2 Curr token: " + this.currentToken);
                this.CurrentState = nextState;
                return CurrentState;
            }

            if (nextState == LexicalMachineState.EMPTY && this.CurrentState == LexicalMachineState.VAR_TOKEN) {
                Console.WriteLine("3 Curr token: " + this.currentToken);
                this.UpdateTokenState(command);
                Console.WriteLine("4 Curr token: " + this.currentToken);
                this.OnTokenIdentified(command);
                this.ClearToken();

                this.CurrentState = nextState;
                return CurrentState;
            }

            if (nextState == LexicalMachineState.EMPTY && command.Category == AtomType.CONTROL ||
                nextState == LexicalMachineState.EMPTY && this.CurrentState != LexicalMachineState.EMPTY ||
                nextState == LexicalMachineState.SPECIAL_TOKEN && 
                (this.CurrentState == LexicalMachineState.STRING_TOKEN || this.CurrentState == LexicalMachineState.INT_TOKEN) ||
                (nextState == LexicalMachineState.INT_TOKEN || nextState == LexicalMachineState.STRING_TOKEN) && 
                this.CurrentState == LexicalMachineState.SPECIAL_TOKEN) {
                
                this.OnTokenIdentified(command);
                this.ClearToken();
            }

            this.UpdateTokenState(command);
            this.CurrentState = nextState;
            return CurrentState;
        }

        protected virtual void OnTokenIdentified(AsciiAtom command) {
            
        }

        class PrintStateTransition {

            public PrintMachineState CurrentState { get; private set; }
            public TokenType Command { get; private set; }

            public LexicalStateTransition(PrintMachineState currentState, TokenType command)
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
                PrintStateTransition other = obj as PrintStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }

        }
    }
}