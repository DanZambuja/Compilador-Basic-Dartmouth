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

    public class PrintStateMachine : ISubStateMachine {
        private Categorizer categorizer = new Categorizer();
        private Dictionary<PrintStateTransition, PrintMachineState> transitions;
        public event TokenIdentified NotifyTokenIdentified;
        private string currentToken = string.Empty;
        private int indexOrSize = 0;
        public PrintMachineState CurrentState { get; private set; }

        public PrintStateMachine() {
            CurrentState = PrintMachineState.START;
            transitions = new Dictionary<LexicalStateTransition, LexicalMachineState>
            {
                { new PrintStateTransition(PrintMachineState.START, TokenType.PRINT), PrintMachineState.PRINT },

                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.STRING), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.VAR), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.INT), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.ARRAY), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.END), PrintMachineState.START },

                { new PrintStateTransition(PrintMachineState.PRINT_MULTIPLE, TokenType.DELIMITER), PrintMachineState.PRINT },
                { new PrintStateTransition(PrintMachineState.PRINT_MULTIPLE, TokenType.END), PrintMachineState.START },
            };
        }

        public void ConsumeCategorizedSymbolEvent(AsciiAtom symbol) {
            this.MoveNext(symbol);
        }

        private void ClearToken() {
            this.currentToken = string.Empty;
        }

        public PrintMachineState GetNext(TokenType token)
        {
            PrintStateTransition transition = new PrintStateTransition(CurrentState, token);
            PrintMachineState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + token);

            return nextState;
        }

        public PrintMachineState MoveToNextState(TokenType token)
        {
            PrintMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
            return CurrentState;
        }

        class PrintStateTransition {

            public PrintMachineState CurrentState { get; private set; }
            public TokenType Command { get; private set; }

            public PrintStateTransition(PrintMachineState currentState, TokenType command)
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