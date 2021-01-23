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
            if (token.Type != TokenType.TO) {
                string instruction = string.Empty;

                instruction += "    b LABEL_" + token.Text + "\n";

                this.fileManager.WriteInstructionsToFile(instruction);
            }
        }
    }
}