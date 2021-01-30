using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class EndCommand
    {
        private FileManager fileManager;
        public EndCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }
        public void EndInstruction() {
            string instructions = string.Empty;

            instructions += "   b END\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}