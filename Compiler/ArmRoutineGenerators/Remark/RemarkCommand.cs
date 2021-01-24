using FileIO;
using Compiler.LexicalAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public class RemarkCommand : ICommand
    {
        private FileManager fileManager;

        public RemarkCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ConsumeToken(Token token) { }
    }
}