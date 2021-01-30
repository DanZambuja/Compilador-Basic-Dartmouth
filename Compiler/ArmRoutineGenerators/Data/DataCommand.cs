using System;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class DataCommand : ICommand
    {
        private readonly FileManager fileManager;
        private readonly VariableTable variables;
        public DataCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }
        public void ConsumeToken(Token token) {
            string instructions = string.Empty;

            if (this.variables.readVariables.Count > 0) {

                instructions += "   ldr r0, =" + token.Text + "\n";

                Token element = this.variables.readVariables.Dequeue() as Token;
                if (element.Type == TokenType.VAR) {
                    instructions += "   ldr r1, =" + this.variables.variableToIndex[element.Text] + "\n";
                } else if (element.Type == TokenType.ARRAY) {
                    int index = this.variables.variableToIndex[element.Text] + element.IndexOrSize;
                    instructions += "   ldr r1, =" + index + "\n";
                }
                
                instructions += "   adr r2, mem\n";
                instructions += "   ldr r3, =4\n";
                instructions += "   mul r5, r1, r3\n";
                instructions += "   add r2, r2, r5\n";
                instructions += "   str r0, [r2]\n";
            }

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}