using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class LetSecondArithmeticCommand
    {
        private readonly FileManager fileManager;
        private Token operation;

        public LetSecondArithmeticCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ReceiveOperation(Token token) {
            this.operation = token;
        }

        public void ReceiveRightSideOfOperation(Token token) {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + token.Text + "\n";
            if (this.operation.Type == TokenType.PLUS) {
                instructions += "   add r0, r0, r1\n";
            } else if (this.operation.Type == TokenType.MINUS) {
                instructions += "   sub r0, r0, r1\n";
            } else if (this.operation.Type == TokenType.MULT) {
                instructions += "   mul r4, r0, r1\n";
                instructions += "   mov r0, r4, r1\n";
            } else if (this.operation.Type == TokenType.DIV) {
                throw new Exception("Division not implemented!");
            } else {
                throw new Exception("Failed to construct operation!");
            }

            this.fileManager.WriteInstructionsToFile(instructions);
        }

    }
}