using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class FinalizationCommand
    {
        public string AllocateMemorySpaceForVariables(int variableCounter) {

            string armCommand = string.Empty;
            armCommand = "mem:\n";
            armCommand += ".space " + 4 * variableCounter;

            return armCommand;
        }
    }
}