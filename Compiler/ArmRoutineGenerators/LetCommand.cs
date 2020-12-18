using System;
using FileIO;
using Compiler.LexicalAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public class LetCommand
    {
        private int variableIndex;
        private FileManager fileManager;

        public LetCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ReceiveVariableIndex(int index) {
            this.variableIndex = index;
        }

        public void GenerateStoreInMemInstructions() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + this.variableIndex + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}