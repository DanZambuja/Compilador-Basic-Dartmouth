using System;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class ForCommand
    {
        private readonly FileManager fileManager;
        private readonly VariableTable variables;
        public ForCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }
        public void ReceivedLoopedVariable(Token loopedVariable) {
            // Check if variable or array element
            switch(loopedVariable.Type) {
                case TokenType.VAR:
                    if (!this.variables.variableToIndex.ContainsKey(loopedVariable.Text)) { // if variable used to loop over is not already declared, make space in memory and add to index
                        this.variables.variableToIndex[loopedVariable.Text] = this.variables.variableCounter++;
                    }
                    break;
                case TokenType.ARRAY:
                    if (!this.variables.variableToIndex.ContainsKey(loopedVariable.Text))
                        throw new Exception("Array element has to be of an already declared array in order to be used as looping element!s");
                    break;
                default:
                    throw new Exception("UnexpectedType!\nExpected variable to loop over in for command!");
            }

            int variableIndex = this.variables.variableToIndex[loopedVariable.Text];
            if (loopedVariable.Type == TokenType.ARRAY) {
                variableIndex += loopedVariable.IndexOrSize;
            }
            // starting value for expression would be on r0
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + variableIndex + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        public void SaveMaxValueForLoop(Token loopedVariable) {
            string loopedVariableMaxValueName = string.Empty;
            if (loopedVariable.Type == TokenType.VAR) {
                loopedVariableMaxValueName = loopedVariable.Text + "_MAX_LOOP";
            } else if (loopedVariable.Type == TokenType.ARRAY) {
                loopedVariableMaxValueName = loopedVariable.Text + "_" + loopedVariable.IndexOrSize + "_MAX_LOOP";
            }
            
            if (!this.variables.variableToIndex.ContainsKey(loopedVariableMaxValueName)) {
                this.variables.variableToIndex[loopedVariableMaxValueName] = this.variables.variableCounter++;
            }

            this.variables.variableToLoopStart[loopedVariableMaxValueName] = this.variables.currentProgramLine + 10;
            
            // Result of expression for max value of loop would be r0
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + this.variables.variableToIndex[loopedVariableMaxValueName] + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        public void SaveStepValueForLoop(Token loopedVariable) {
            string loopedVariableStepValueName = string.Empty;
            if (loopedVariable.Type == TokenType.VAR) {
                loopedVariableStepValueName = loopedVariable.Text + "_STEP_LOOP";
            } else if (loopedVariable.Type == TokenType.ARRAY) {
                loopedVariableStepValueName = loopedVariable.Text + "_" + loopedVariable.IndexOrSize + "_STEP_LOOP";
            }
            
            if (!this.variables.variableToIndex.ContainsKey(loopedVariableStepValueName)) {
                this.variables.variableToIndex[loopedVariableStepValueName] = this.variables.variableCounter++;
            }
            
            // Result of expression for max value of loop would be r0
            string instructions = string.Empty;

            instructions += "   ldr r1, =" + this.variables.variableToIndex[loopedVariableStepValueName] + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}