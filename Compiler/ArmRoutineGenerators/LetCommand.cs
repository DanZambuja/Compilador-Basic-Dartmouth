using System;
using FileIO;
using Compiler.LexicalAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public class LetCommand : ICommand
    {
        private int variableIndex;
        private int attributedValue;
        private FileManager fileManager;

        public LetCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ConsumeToken(Token token) {

        }
    }
}