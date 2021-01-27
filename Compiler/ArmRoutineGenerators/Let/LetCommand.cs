using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class LetCommand
    {
        private FileManager fileManager;

        public LetCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        // Actual value expected to be on r0
        public void GenerateStoreInMemInstructions(int index) {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + index + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}