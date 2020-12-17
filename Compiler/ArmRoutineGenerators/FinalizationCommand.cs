using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class FinalizationCommand
    {
        private FileManager fileManager;
        public FinalizationCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }
        public void AllocateMemorySpaceForVariables(int variableCounter) {
            string data = string.Empty;
            if (variableCounter > 0) {
                data = "mem:\n";
                data += " .space " + 4 * variableCounter;

                this.fileManager.WriteVarAndDataTable(data);
            }
        }
    }
}