using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public class ReturnCommand
    {
        private readonly FileManager fileManager;
        private readonly VariableTable variables;

        public ReturnCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }
        public void ReturnInstructions() {
            string instruction = string.Empty;

            instruction += "    b LABEL_" + this.variables.lastGoSubCalled + "\n";

            this.fileManager.WriteInstructionsToFile(instruction);
        }
    }
}