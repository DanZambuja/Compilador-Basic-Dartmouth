using FileIO;

namespace Project.Compiler.ArmRoutineGenerators
{
    public class FinalizationCommand
    {
        private int variableCounter;

        public FinalizationCommand(int variableCounter) {
            this.variableCounter = variableCounter;
        }

        public string AllocateMemorySpaceForVariables() {

            string armCommand = string.Empty;
            armCommand = "mem:\n";
            armCommand += ".space " + 4 * this.variableCounter;

            return armCommand;
        }
    }
}