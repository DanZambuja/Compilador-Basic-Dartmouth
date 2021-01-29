using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class DefCommand : ICommand
    {
        private readonly FileManager fileManager;
        public DefCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }
        public void ConsumeToken(Token token) {

        }
    }
}