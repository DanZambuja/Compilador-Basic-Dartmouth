using System;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum PrintMachineState
    {
        START,
        PRINT,
        PRINT_MULTIPLE
    }

    public class PrintStateMachine : ISubStateMachine {
        private Dictionary<PrintStateTransition, PrintMachineState> transitions;
        public PrintMachineState CurrentState { get; private set; }
        private VariableTable variables;
        private PrintCommand command;

        public PrintStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = PrintMachineState.START;
            transitions = new Dictionary<PrintStateTransition, PrintMachineState>
            {
                { new PrintStateTransition(PrintMachineState.START, TokenType.END), PrintMachineState.START },
                { new PrintStateTransition(PrintMachineState.START, TokenType.PRINT), PrintMachineState.PRINT },

                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.STRING), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.QUOTED_STRING), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.VAR), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.INT), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.ARRAY), PrintMachineState.PRINT_MULTIPLE },
                { new PrintStateTransition(PrintMachineState.PRINT, TokenType.END), PrintMachineState.START },

                { new PrintStateTransition(PrintMachineState.PRINT_MULTIPLE, TokenType.COMMA), PrintMachineState.PRINT },
                { new PrintStateTransition(PrintMachineState.PRINT_MULTIPLE, TokenType.END), PrintMachineState.START },
            };
            this.variables = variables;
            this.command = new PrintCommand(fileManager);
        }


        public PrintMachineState GetNext(Token token)
        {
            PrintMachineState nextState = this.CurrentState;
            PrintStateTransition transition = new PrintStateTransition(CurrentState, token.Type);

            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (this.CurrentState == PrintMachineState.PRINT && token.Type != TokenType.END) {
                this.command.ConsumeToken(token);
            }

            Console.WriteLine("P: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            PrintMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class PrintStateTransition {

            public PrintMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public PrintStateTransition(PrintMachineState currentState, TokenType token)
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
                PrintStateTransition other = obj as PrintStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}