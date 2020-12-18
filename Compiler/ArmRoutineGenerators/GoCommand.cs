using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class GoCommand : ICommand
    {
        private readonly FileManager fileManager;
        public GoCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ConsumeToken(Token token) {
            string instruction = string.Empty;

            instruction += "    JP LABEL_" + token.Text + "\n";

            this.fileManager.WriteInstructionsToFile(instruction);
        }
    }
}