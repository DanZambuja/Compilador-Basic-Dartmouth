using System;
using Compiler.AsciiCategorizer;
using System.Collections.Generic;

namespace Compiler.LexicalAnalysis
{
    public enum LexicalMachineState
    {
        EMPTY,
        INT_TOKEN,
        STRING_TOKEN,
        QUOTED_STRING_TOKEN,
        SPECIAL_TOKEN,
        ARRAY_TOKEN,
        VAR_TOKEN
    }

    public enum TokenType {
        PRINT,
        DIM,
        LET,
        READ,
        DATA,
        FOR,
        TO,
        NEXT,
        GO, 
        GOTO,
        GOSUB,
        RETURN,
        REMARK,
        IF,
        THEN,
        STEP,
        COMMA,
        STRING,
        QUOTED_STRING,
        VAR,
        ARRAY,
        ARRAY_ELEMENT,
        INT,
        EQUALS,
        PLUS,
        MINUS,
        MULT,
        DIV,
        OPENING_BRACES,
        CLOSING_BRACES,
        POWER,
        CALCULATED_RESULT,
        ERROR,
        END
    }

    public class Token {
        public TokenType Type { get; private set; }
        public string Text { get; set; }
        public int IndexOrSize {get; set;}

        public Token(LexicalMachineState state, string tokenString, AsciiAtom command) {
            this.Text = tokenString;

            if (state == LexicalMachineState.STRING_TOKEN) {
                this.Type = this.Text.ToUpper() switch 
                {
                    "PRINT" => TokenType.PRINT,
                    "DIM" => TokenType.DIM,
                    "LET" => TokenType.LET,
                    "READ" => TokenType.READ,
                    "DATA" => TokenType.DATA,
                    "FOR" => TokenType.FOR,
                    "TO" => TokenType.TO,
                    "NEXT" => TokenType.NEXT,
                    "GO" => TokenType.GO,
                    "GOTO" => TokenType.GOTO,
                    "GOSUB" => TokenType.GOSUB,
                    "RETURN" => TokenType.RETURN,
                    "REM" => TokenType.REMARK,
                    "IF" => TokenType.IF,
                    "STEP" => TokenType.STEP,
                    "THEN" => TokenType.THEN,
                    _ => TokenType.STRING
                };

            } else if (state == LexicalMachineState.INT_TOKEN) {
                this.Type = TokenType.INT;

            } else if (state == LexicalMachineState.SPECIAL_TOKEN) {
                this.Type = this.Text switch 
                {
                    "=" => TokenType.EQUALS,
                    ":=" => TokenType.EQUALS,
                    "(" => TokenType.OPENING_BRACES,
                    ")" => TokenType.CLOSING_BRACES,
                    "," => TokenType.COMMA,
                    "+" => TokenType.PLUS,
                    "-" => TokenType.MINUS,
                    "*" => TokenType.MULT,
                    "/" => TokenType.DIV,
                    _ => TokenType.ERROR
                };
            } else if (state == LexicalMachineState.VAR_TOKEN) {
                this.Type = TokenType.VAR;
            } else if (state == LexicalMachineState.QUOTED_STRING_TOKEN) {
                this.Type = TokenType.QUOTED_STRING;
            } else if (command.Category == AtomType.CONTROL) {
                this.Type = TokenType.END;
            }else {
                this.Type = TokenType.ERROR;
            }
        }
        public Token(string vectorVariable, int indexOrSize, bool elementOfArray) {
            this.Text = vectorVariable;
            this.IndexOrSize = indexOrSize;
            if (elementOfArray)
                this.Type = TokenType.ARRAY_ELEMENT;
            else
                this.Type = TokenType.ARRAY;
        }

        public Token() {
            this.Text = string.Empty;
            this.Type = TokenType.END;
        }

        public Token(TokenType type) {
            this.Text = string.Empty;
            this.Type = TokenType.CALCULATED_RESULT;
        }
    }
    
    public delegate void TokenIdentified(Token token);

    public class LexicalStateMachine {
        private Categorizer categorizer = new Categorizer();
        private Dictionary<LexicalStateTransition, LexicalMachineState> transitions;
        public event TokenIdentified NotifyTokenIdentified;
        private string currentToken = string.Empty;
        private int indexOrSize = 0;
        public LexicalMachineState CurrentState { get; private set; }

        public LexicalStateMachine() {
            CurrentState = LexicalMachineState.EMPTY;
            transitions = new Dictionary<LexicalStateTransition, LexicalMachineState>
            {
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.DIGIT), LexicalMachineState.INT_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.LETTER), LexicalMachineState.VAR_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.QUOTE), LexicalMachineState.QUOTED_STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.OPENING_PAR), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.CLOSING_PAR), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.DELIMITER), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.EMPTY, AtomType.CONTROL), LexicalMachineState.EMPTY },
                

                { new LexicalStateTransition(LexicalMachineState.VAR_TOKEN, AtomType.DIGIT), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.VAR_TOKEN, AtomType.DELIMITER), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.VAR_TOKEN, AtomType.LETTER), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.VAR_TOKEN, AtomType.OPENING_PAR), LexicalMachineState.ARRAY_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.VAR_TOKEN, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN },
                
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.DIGIT), LexicalMachineState.INT_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.OPENING_PAR), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.CLOSING_PAR), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.DELIMITER), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.INT_TOKEN, AtomType.CONTROL), LexicalMachineState.EMPTY },

                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.LETTER), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.DIGIT), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.QUOTE), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.OPENING_PAR), LexicalMachineState.ARRAY_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN},
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.CONTROL), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.STRING_TOKEN, AtomType.DELIMITER), LexicalMachineState.EMPTY },

                { new LexicalStateTransition(LexicalMachineState.QUOTED_STRING_TOKEN, AtomType.DIGIT), LexicalMachineState.QUOTED_STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.QUOTED_STRING_TOKEN, AtomType.LETTER), LexicalMachineState.QUOTED_STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.QUOTED_STRING_TOKEN, AtomType.DELIMITER), LexicalMachineState.QUOTED_STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.QUOTED_STRING_TOKEN, AtomType.SPECIAL), LexicalMachineState.QUOTED_STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.QUOTED_STRING_TOKEN, AtomType.QUOTE), LexicalMachineState.EMPTY },

                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.LETTER), LexicalMachineState.STRING_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.DIGIT), LexicalMachineState.INT_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.SPECIAL), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.OPENING_PAR), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.CLOSING_PAR), LexicalMachineState.SPECIAL_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.DELIMITER), LexicalMachineState.EMPTY },
                { new LexicalStateTransition(LexicalMachineState.SPECIAL_TOKEN, AtomType.CONTROL), LexicalMachineState.EMPTY },

                { new LexicalStateTransition(LexicalMachineState.ARRAY_TOKEN, AtomType.DIGIT), LexicalMachineState.ARRAY_TOKEN },
                { new LexicalStateTransition(LexicalMachineState.ARRAY_TOKEN, AtomType.CLOSING_PAR), LexicalMachineState.EMPTY },
            };
        }

        public void ConsumeCategorizedSymbolEvent(AsciiAtom symbol) {
            this.MoveNext(symbol);
        }

        private void UpdateTokenState(AsciiAtom command) {
            if (command.Category != AtomType.CONTROL && 
                command.Category != AtomType.DELIMITER) {
                if (this.CurrentState == LexicalMachineState.ARRAY_TOKEN) {
                    this.indexOrSize = Int32.Parse(command.Symbol.ToString());
                } 
                else {
                    this.currentToken = this.currentToken + command.Symbol;
                }
            } else if (this.CurrentState == LexicalMachineState.QUOTED_STRING_TOKEN) {
                this.currentToken = this.currentToken + command.Symbol;
            }

        }

        private void ClearToken() {
            this.currentToken = string.Empty;
        }

        public LexicalMachineState GetNext(AtomType command)
        {
            LexicalMachineState nextState = this.CurrentState;
            LexicalStateTransition transition = new LexicalStateTransition(CurrentState, command);
            
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);

            Console.WriteLine("Lexical :" + this.CurrentState + " -> " + nextState + " : " + command.ToString());
            
            return nextState;
        }

        public LexicalMachineState MoveNext(AsciiAtom command)
        {
            LexicalMachineState nextState = GetNext(command.Category);

            if (nextState == LexicalMachineState.VAR_TOKEN && this.CurrentState == LexicalMachineState.EMPTY) {
                this.UpdateTokenState(command);
                this.CurrentState = nextState;
                return CurrentState;
            }

            if (nextState == LexicalMachineState.EMPTY && this.CurrentState == LexicalMachineState.VAR_TOKEN ||
                nextState == LexicalMachineState.EMPTY && this.CurrentState == LexicalMachineState.QUOTED_STRING_TOKEN) {
                this.UpdateTokenState(command);
                this.OnTokenIdentified(command);
                this.ClearToken();

                this.CurrentState = nextState;
                return CurrentState;
            }

            if (nextState == LexicalMachineState.EMPTY && command.Category == AtomType.CONTROL ||
                nextState == LexicalMachineState.EMPTY && this.CurrentState != LexicalMachineState.EMPTY ||
                nextState == LexicalMachineState.SPECIAL_TOKEN && 
                (this.CurrentState == LexicalMachineState.STRING_TOKEN || this.CurrentState == LexicalMachineState.INT_TOKEN || this.CurrentState == LexicalMachineState.SPECIAL_TOKEN) ||
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
            if (this.CurrentState == LexicalMachineState.ARRAY_TOKEN) {
                NotifyTokenIdentified?.Invoke(new Token(this.currentToken, this.indexOrSize, true));
            } else if (command.Category == AtomType.CONTROL) {
                NotifyTokenIdentified?.Invoke(new Token(this.CurrentState, this.currentToken, command));
                NotifyTokenIdentified?.Invoke(new Token());
            }
            else {
                Console.WriteLine("L: sent token: " + this.currentToken);
                NotifyTokenIdentified?.Invoke(new Token(this.CurrentState, this.currentToken, command));
            }
        }
        
        public void End() {
            NotifyTokenIdentified?.Invoke(new Token());
        }

        class LexicalStateTransition {

            public LexicalMachineState CurrentState { get; private set; }
            public AtomType Command { get; private set; }

            public LexicalStateTransition(LexicalMachineState currentState, AtomType command)
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
                LexicalStateTransition other = obj as LexicalStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }

        }
    }
}