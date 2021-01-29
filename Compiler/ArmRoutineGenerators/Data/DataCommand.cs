using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class DataCommand : ICommand
    {
        private readonly FileManager fileManager;
        public DataCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }
        public void ConsumeToken(Token token) {

        }
    }
}