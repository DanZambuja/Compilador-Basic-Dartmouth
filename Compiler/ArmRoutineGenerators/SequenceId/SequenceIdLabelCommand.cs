using System;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class SequenceIdLabelCommand : ICommand
    {

        private readonly FileManager fileManager;
        private readonly VariableTable variables;

        public SequenceIdLabelCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }

        public void ConsumeToken(Token token) {
            this.variables.currentProgramLine = int.Parse(token.Text);

            string label = "LABEL_" + token.Text + ":\n";

            this.fileManager.WriteInstructionsToFile(label);
        }
    }
}