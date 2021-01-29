using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class ReadCommand : ICommand
    {
        private readonly FileManager fileManager;
        public ReadCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }
        public void ConsumeToken(Token token) {

        }
    }
}