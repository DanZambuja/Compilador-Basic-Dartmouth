using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class LetFirstArithmeticCommand
    {
        private readonly FileManager fileManager;

        public LetFirstArithmeticCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ReceivedLeftSideOfOperation(Token token) {
            string instruction = string.Empty;
            instruction += "    ldr r0, =" + token.Text + "\n";
            this.fileManager.WriteInstructionsToFile(instruction);
        }

    }
}