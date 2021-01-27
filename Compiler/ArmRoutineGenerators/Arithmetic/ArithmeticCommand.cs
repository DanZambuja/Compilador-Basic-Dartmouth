using FileIO;
using Compiler.SyntaxAnalysis;
using Compiler.LexicalAnalysis;
using System.Collections;

namespace Compiler.ArmRoutineGenerators
{
    public class ArithmeticCommand
    {
        private VariableTable variables;
        private FileManager fileManager;
        private Stack stack;
        private Queue output;

        public ArithmeticCommand(VariableTable variables, FileManager fileManager) {
            this.variables = variables;
            this.fileManager = fileManager;
            this.stack = new Stack();
            this.output = new Queue();
        }

        public void ConsumeToken(Token token) {
            
        }

        public void EndOfExpression() {
            this.stack.Clear();
            this.output.Clear();
        }

        private void Sum() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        private void Sub() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        private void Mult() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
        private void Div() {
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}