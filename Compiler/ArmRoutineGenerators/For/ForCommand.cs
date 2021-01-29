using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class ForCommand : ICommand
    {
        private readonly FileManager fileManager;
        public ForCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }
        public void ConsumeToken(Token token) {

        }
    }
}