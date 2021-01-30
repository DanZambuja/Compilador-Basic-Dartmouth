using System;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class IfCommand
    {
        private readonly FileManager fileManager;
        private readonly VariableTable variables;
        public IfCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }

        public void MoveResultToSecondRegister() {
            string instruction = "  mov r1, r0\n";
            this.fileManager.WriteInstructionsToFile(instruction);
        }

        public void IfInstructions(int destination, Token comparison) {
            string instruction = string.Empty;

            instruction += "    cmp r1, r0" + "\n";
            instruction += comparison.Type switch {
                TokenType.EQUALS            => "    beq LABEL_" + destination + "\n",
                TokenType.NOT_EQUAL         => "    bne LABEL_" + destination + "\n",
                TokenType.GREATER           => "    bgt LABEL_" + destination + "\n",
                TokenType.GREATER_OR_EQUAL  => "    bge LABEL_" + destination + "\n",
                TokenType.LESS              => "    blt LABEL_" + destination + "\n",
                TokenType.LESS_OR_EQUAL     => "    ble LABEL_" + destination + "\n",
                _ => throw new Exception("Comparator not of correct Type!")
            };

            this.fileManager.WriteInstructionsToFile(instruction);
        }
    }
}