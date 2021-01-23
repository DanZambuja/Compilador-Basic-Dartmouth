using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class SequenceIdLabelCommand : ICommand
    {

        private readonly FileManager fileManager;

        public SequenceIdLabelCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ConsumeToken(Token token) {
            string label = string.Empty;
            label += "LABEL_" + token.Text + ":\n";

            this.fileManager.WriteInstructionsToFile(label);
        }
    }
}