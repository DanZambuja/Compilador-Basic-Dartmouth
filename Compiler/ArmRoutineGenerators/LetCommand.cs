using System;
using FileIO;
using Compiler.LexicalAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public class LetCommand : ICommand
    {
        private int variableIndex;
        private int attributedValue;
        private string armInstructions = string.Empty;
        private FileManager fileManager;

        public LetCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ConsumeToken(Token token) {
            this.armInstructions += token.Text;
        }

        public string GenerateArmInstructions() {
            return this.armInstructions;
        }

        public void Clear() {
            Console.WriteLine(this.armInstructions);
            this.armInstructions = string.Empty;
        }
    }
}