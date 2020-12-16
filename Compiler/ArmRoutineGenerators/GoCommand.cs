using System;
using Compiler.LexicalAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public class GoCommand : ICommand
    {
        private string armInstructions = string.Empty;

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