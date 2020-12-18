using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class ArithmeticCommand : ICommand
    {
        private readonly FileManager fileManager;
        public ArithmeticCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }
        
        public void ConsumeToken(Token token) {

        }
    }
}