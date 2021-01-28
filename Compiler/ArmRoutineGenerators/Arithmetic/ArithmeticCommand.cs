using System;
using FileIO;
using Compiler.SyntaxAnalysis;
using Compiler.LexicalAnalysis;
using Compiler.Exceptions;
using System.Collections;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    class Precedence {
        public int Value { get; private set; }
        public int Associativeness { get; private set; }
        // 0: left 1: right
        public Precedence(int value, int associativeness) {
            this.Value = Value;
            this.Associativeness = associativeness;
        }
    }

    public class ArithmeticCommand
    {
        private Dictionary<TokenType, Precedence> precedenceTable = new Dictionary<TokenType, Precedence> {
            { TokenType.PLUS, new Precedence(2, 0) },
            { TokenType.MINUS, new Precedence(2, 0) },
            { TokenType.MULT, new Precedence(3, 0) },
            { TokenType.DIV, new Precedence(3, 0) },
            { TokenType.POWER, new Precedence(4, 1) },
        };
        private VariableTable variables;
        private FileManager fileManager;
        private Stack operatorS;
        private Queue outputQ;

        public ArithmeticCommand(VariableTable variables, FileManager fileManager) {
            this.variables = variables;
            this.fileManager = fileManager;
            this.operatorS = new Stack();
            this.outputQ = new Queue();
        }

        // Use Shunting Yard to convert to Reverse Polish Notation
        public void ConsumeToken(Token token) {
            if (token.Type == TokenType.END) {
                while (this.operatorS.Count >= 0) {
                    this.outputQ.Enqueue(this.operatorS.Pop());
                }
                this.Evaluate();
            } else if (this.IsNumber(token)) {
                this.outputQ.Enqueue(token);
            } else if (this.IsFunction(token)) {
                this.operatorS.Push(token);
            } else if (this.IsOperator(token)) {
                if (this.operatorS.Count > 0) {
                    Token topOfStack = this.operatorS.Peek() as Token;
                    while (this.operatorS.Count > 0 && topOfStack.Type != TokenType.OPENING_BRACES &&
                          ((this.precedenceTable[topOfStack.Type].Value > this.precedenceTable[token.Type].Value) || (this.precedenceTable[topOfStack.Type].Value == this.precedenceTable[token.Type].Value && this.precedenceTable[token.Type].Associativeness == 0))) {
                        this.outputQ.Enqueue(this.operatorS.Pop());
                    }
                }
                this.operatorS.Push(token);
                
            } else if (token.Type == TokenType.OPENING_BRACES) {
                this.operatorS.Push(token);
            } else if (token.Type == TokenType.CLOSING_BRACES) {
                if (this.operatorS.Count > 0) {
                    for (Token topOfStack = this.operatorS.Peek() as Token; topOfStack.Type != TokenType.OPENING_BRACES; topOfStack = this.operatorS.Peek() as Token) {
                        this.outputQ.Enqueue(this.operatorS.Pop());
                    }
                }
                if (this.operatorS.Count > 0 && (this.operatorS.Peek() as Token).Type == TokenType.OPENING_BRACES) {
                    this.operatorS.Pop();
                } else {
                    throw new MismatchedBracesException("Mismatched braces");
                }
                if (this.operatorS.Count > 0 && this.IsFunction((this.operatorS.Peek() as Token))) {
                    this.outputQ.Enqueue(this.operatorS.Pop());
                }
            } 
        }

        private bool IsOperator(Token token) {
            if (token.Type == TokenType.MINUS || token.Type == TokenType.PLUS || token.Type == TokenType.MULT || token.Type == TokenType.DIV || token.Type == TokenType.POWER)
                return true;
            else
                return false;
        }

        private bool IsFunction(Token token) {
            return false;
        }

        private bool IsNumber(Token token) {
            if (token.Type == TokenType.INT || token.Type == TokenType.VAR || token.Type == TokenType.ARRAY_ELEMENT)
                return true;
            else
                return false;
        }

        private void Evaluate() {
             Console.WriteLine();
             foreach(Token el in this.outputQ) {
                 Console.Write(el.Text + " ");
             }
             Console.WriteLine();
        }

        public void EndOfExpression() {
            while (this.operatorS.Count > 0) {
                    this.outputQ.Enqueue(this.operatorS.Pop());
            }
            this.Evaluate();
            this.operatorS.Clear();
            this.outputQ.Clear();
        }

        private void Sum() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        private void Sub() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        private void Mult() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
        private void Div() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}