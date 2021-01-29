using System;
using Compiler.LexicalAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class GoSubCommand : ICommand
    {
        private readonly FileManager fileManager;
        public GoSubCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }
        public void ConsumeToken(Token token) {

        }
    }
}