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
        private Stack rpn;
        private Queue outputQ;

        public ArithmeticCommand(VariableTable variables, FileManager fileManager) {
            this.variables = variables;
            this.fileManager = fileManager;
            this.operatorS = new Stack();
            this.rpn = new Stack();
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

        private void RemoveRemainingOperators() {
            while (this.operatorS.Count > 0) {
                    this.outputQ.Enqueue(this.operatorS.Pop());
            }
        }

        public void EndOfExpression() {
            this.RemoveRemainingOperators();
            this.ShowOutputContents();
            this.Evaluate();
            this.operatorS.Clear();
            this.outputQ.Clear();
        }

        private void ShowOutputContents() {
            Console.WriteLine();
            foreach (Token token in this.outputQ) {
                Console.Write(token.Text + " ");
            }
            Console.WriteLine();
        }

        private void Evaluate() {
            if (this.outputQ.Count == 1) {
                Token value = this.outputQ.Dequeue() as Token;
                this.SingleValueAttribuition(value);
            } else if (this.outputQ.Count > 1) {
                int acc = 0;
                while (this.outputQ.Count > 1) {
                    Token token = this.outputQ.Dequeue() as Token;
                    if (this.IsOperator(token) || this.IsFunction(token)) {
                        Token first = this.rpn.Pop() as Token;
                        Token second = this.rpn.Pop() as Token;
                        this.CreateInstructions(first, second, token, acc);
                        acc--;
                        this.rpn.Push(new Token(TokenType.CALCULATED_RESULT));
                    } else if (this.IsNumber(token)){
                        acc++;
                        this.rpn.Push(token);
                    }
                }
                this.GetResultToMainRegister();
            } 
        }

        private void CreateInstructions(Token first, Token second, Token operation, int accumulator) {
            string instructions = string.Empty;
            string[] registers = this.GetCurrentRegisters(accumulator);

            switch (first.Type) {
                case TokenType.INT:
                    instructions += "   ldr " + registers[1] + ", =" + first.Text + "\n";
                    break;
                case TokenType.VAR:
                    instructions += "   ldr r1, =" + this.variables.variableToIndex[first.Text] + "\n";
                    instructions += "   adr r2, mem\n";
                    instructions += "   ldr r3, =4\n";
                    instructions += "   mul r5, r1, r3\n";
                    instructions += "   add r2, r2, r5\n";
                    instructions += "   ldr " + registers[1] + ", [r2]\n";
                    break;
                case TokenType.ARRAY_ELEMENT:
                    instructions += "   ldr r1, =" + this.variables.variableToIndex[first.Text] + "\n";
                    instructions += "   adr r2, mem\n";
                    instructions += "   ldr r3, =4\n";
                    instructions += "   mul r5, r1, r3\n";
                    instructions += "   add r2, r2, r5\n";
                    instructions += "   ldr " + registers[1] + ", [r2]\n";
                    break;
                case TokenType.CALCULATED_RESULT:
                    break;
            }

            switch (second.Type) {
                case TokenType.INT:
                    instructions += "   ldr " + registers[0] + ", =" + second.Text + "\n";
                    break;
                case TokenType.VAR:
                    instructions += "   ldr r1, =" + this.variables.variableToIndex[second.Text] + "\n";
                    instructions += "   adr r2, mem\n";
                    instructions += "   ldr r3, =4\n";
                    instructions += "   mul r5, r1, r3\n";
                    instructions += "   add r2, r2, r5\n";
                    instructions += "   ldr " + registers[0] + ", [r2]\n";
                    break;
                case TokenType.ARRAY_ELEMENT:
                    instructions += "   ldr r1, =" + this.variables.variableToIndex[second.Text] + "\n";
                    instructions += "   adr r2, mem\n";
                    instructions += "   ldr r3, =4\n";
                    instructions += "   mul r5, r1, r3\n";
                    instructions += "   add r2, r2, r5\n";
                    instructions += "   ldr " + registers[0] + ", [r2]\n";
                    break;
                case TokenType.CALCULATED_RESULT:
                    break;
            }

            switch (operation.Type) {
                case TokenType.PLUS:
                    instructions += "   add r0, " + registers[0] + ", " + registers[1] + "\n";
                    break;
                case TokenType.MINUS:
                    instructions += "   sub r0, " + registers[0] + ", " + registers[1] + "\n";
                    break;
                case TokenType.MULT:
                    instructions += "   mul r0, " + registers[0] + ", " + registers[1] + "\n";
                    break;
                case TokenType.DIV:
                    throw new Exception("Division not yet implemented");
                case TokenType.POWER:
                    throw new Exception("Power not yet implemented");
            }


            string register = this.GetCurrentRegisterForAccumulation(accumulator);
            instructions += "   mov " + register + ", r0\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        private void SingleValueAttribuition(Token value) {
            string instructions = string.Empty;

            switch (value.Type) {
                case TokenType.INT:
                    instructions += "   ldr r0, =" + value.Text + "\n";
                    break;
                case TokenType.VAR:
                    instructions += "   ldr r1, =" + this.variables.variableToIndex[value.Text] + "\n";
                    instructions += "   adr r2, mem\n";
                    instructions += "   ldr r3, =4\n";
                    instructions += "   mul r5, r1, r3\n";
                    instructions += "   add r2, r2, r5\n";
                    instructions += "   ldr r0, [r2]\n";
                    break;
                case TokenType.ARRAY_ELEMENT:
                    instructions += "   ldr r1, =" + this.variables.variableToIndex[value.Text] + "\n";
                    instructions += "   adr r2, mem\n";
                    instructions += "   ldr r3, =4\n";
                    instructions += "   mul r5, r1, r3\n";
                    instructions += "   add r2, r2, r5\n";
                    instructions += "   ldr r0, [r2]\n";
                    break;
            }

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        private void GetResultToMainRegister() {
            string instructions = string.Empty;

            instructions += "   mov r0, r12\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        private string[] GetCurrentRegisters(int accumulator) {
            if (accumulator >= 11) {
                throw new Exception("Arithmetic expression too complex not enough registers!");
            } else if (accumulator < 2){
                throw new Exception("Not enough operands");
            } else {
                int firstRegister = 12 - (accumulator - 2);
                int secondRegister = 12 - (accumulator - 1);

                string[] registers = new string[2];

                registers[0] = "r" + firstRegister;
                registers[1] = "r" + secondRegister;

                return registers; 
            }
        }

        private string GetCurrentRegisterForAccumulation(int accumulator) {
            if (accumulator >= 11) {
                throw new Exception("Arithmetic expression too complex not enough registers!");
            } else if (accumulator < 2){
                throw new Exception("Not enough operands");
            } else {
                int firstRegister = 12 - (accumulator - 2);
                string freeRegister = "r" + firstRegister;
                return freeRegister; 
            }
        }   
    }
}