using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class DimCommand : ICommand
    {
        private readonly FileManager fileManager;
        private readonly VariableTable variables;
        public DimCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }
        public void ConsumeToken(Token token) {
            this.variables.variableToIndex[token.Text] = this.variables.variableCounter;
            this.variables.variableCounter += token.IndexOrSize;
        }
    }
}