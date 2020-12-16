using System;

namespace Compiler.ArmRoutineGenerators
{
    public class ArithmeticCommand
    {
        private int variableIndex;
        private int attributedValue;

        public ArithmeticCommand(int variableIndex, int attributedValue) {
            this.variableIndex = variableIndex;
            this.attributedValue = attributedValue;
        }
    }
}