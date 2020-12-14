using FileIO;

namespace Project.Compiler.ArmRoutineGenerators
{
    public class LetCommand
    {
        private int variableIndex;
        private int attributedValue;

        public LetCommand(int variableIndex, int attributedValue) {
            this.variableIndex = variableIndex;
            this.attributedValue = attributedValue;
        }

        public string SimpleAttribuition() {

            string armCommand = string.Empty;
            armCommand  = "LDR r0, #" + this.variableIndex + "\n";
            armCommand += "MUL r1, r0, #4\n";
            armCommand += "ADR r2, mem\n";
            armCommand += "ADD r3, r2, mem\n";
            armCommand += "LDR r0, #" + this.attributedValue + "\n";
            armCommand += "STR r0, [r3]\n";

            return armCommand;
        }

        public string ArithmeticAttribuition() {
            // r0 has the value of the resulting arithmetic expression
            // which would have been previously generated by the compiler
            // in the case of an arithmetic attribuition
            string armCommand = string.Empty;
            armCommand  = "LDR r4, #" + this.variableIndex + "\n";
            armCommand += "MUL r1, r4, #4\n";
            armCommand += "ADR r2, mem\n";
            armCommand += "ADD r3, r2, mem\n";
            armCommand += "STR r0, [r3]\n";

            return armCommand;
        }


    }
}