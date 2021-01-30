using System;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class GoSubCommand : ICommand
    {
        private readonly FileManager fileManager;
        private readonly VariableTable variables;

        public GoSubCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }
        public void ConsumeToken(Token token) {
            this.variables.lastGoSubCalled = this.variables.currentProgramLine + 10;

            string instructions = string.Empty;
            instructions += "    b LABEL_" + token.Text + "\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}