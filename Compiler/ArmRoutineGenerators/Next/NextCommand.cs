using System;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class NextCommand
    {
        private readonly FileManager fileManager;
        private readonly VariableTable variables;
        public NextCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }
        // 1- É uma variavel que esta sendo loopada? Caso não, indique erro
        // 2- Atualiza valor da variavel loopada
        // 3- Verifica sinal do valor de parada
        // 3.1- se valor de parada é positivo e variavel atualizada <= valor de parada -> loop
        // 3.2 - se valor de parada é negativo e variavel atualizada >= valor de parada -> loop
        public void AddStepToLoopVariableAndLoopBack(Token loopedVariable) {
            string loopedVariableMaxValueName = string.Empty;
            if (loopedVariable.Type == TokenType.VAR) {
                loopedVariableMaxValueName = loopedVariable.Text + "_MAX_LOOP";
            } else if (loopedVariable.Type == TokenType.ARRAY) {
                loopedVariableMaxValueName = loopedVariable.Text + "_" + loopedVariable.IndexOrSize + "_MAX_LOOP";
            }

            string loopedVariableStepValueName = string.Empty;
            if (loopedVariable.Type == TokenType.VAR) {
                loopedVariableStepValueName = loopedVariable.Text + "_STEP_LOOP";
            } else if (loopedVariable.Type == TokenType.ARRAY) {
                loopedVariableStepValueName = loopedVariable.Text + "_" + loopedVariable.IndexOrSize + "_STEP_LOOP";
            }

            if (!this.variables.variableToLoopStart.ContainsKey(loopedVariableMaxValueName)) {
                throw new Exception("Not a variable that is being looped!");
            }

            int variableIndex = this.variables.variableToIndex[loopedVariable.Text];
            if (loopedVariable.Type == TokenType.ARRAY) {
                variableIndex += loopedVariable.IndexOrSize;
            }

            string instructions = string.Empty;

            instructions += "   ldr r1, =" + variableIndex + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   ldr r0, [r2]\n";

            // r0  contém valor da variavel loopada

            instructions += "   ldr r1, =" + this.variables.variableToIndex[loopedVariableMaxValueName] + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   ldr r12, [r2]\n";

            // r12 contém valor de parada

            instructions += "   ldr r1, =" + this.variables.variableToIndex[loopedVariableStepValueName] + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   ldr r11, [r2]\n";

            //r11 contém valor do passo do loop

            string labelForPositiveMaxValue = "PARADA_POSITIVA_" + loopedVariableMaxValueName;
            string labelForNegativeMaxValue = "PARADA_NEGATIVA_" + loopedVariableMaxValueName;
            string labelForOutsideLoopScope = "FIM_LOOP_" + loopedVariableMaxValueName;
            string labelForLoopStart = "LABEL_" + this.variables.variableToLoopStart[loopedVariableMaxValueName];

            // valor da variavel loopada atualizado com passo
            instructions += "   add r0, r0, r11\n"; 

            // Guarda valor atualizado da variavel na memoria
            instructions += "   ldr r1, =" + variableIndex + "\n";
            instructions += "   adr r2, mem\n";
            instructions += "   ldr r3, =4\n";
            instructions += "   mul r5, r1, r3\n";
            instructions += "   add r2, r2, r5\n";
            instructions += "   str r0, [r2]\n";

            // Comparacao da variavel com valor de parada
            instructions += "   cmp r12, #0\n"; // compara valor de parada com 0
            instructions += "   bge " + labelForPositiveMaxValue + "\n";
            instructions += "   b " + labelForNegativeMaxValue + "\n";
            instructions += labelForPositiveMaxValue + ":\n";
            instructions += "   cmp r0, r12\n"; 
            instructions += "   ble " + labelForLoopStart + "\n"; // POSITIVO: Se valor atualizado menor que valor parada volta pro loop
            instructions += "   b " + labelForOutsideLoopScope + "\n";
            instructions += labelForNegativeMaxValue + ":\n";
            instructions += "   cmp r0, r12\n";
            instructions += "   bge " + labelForLoopStart + "\n"; // NEGATIVO: se valor atualizado maior que valor de parada volta pro loop
            instructions += labelForOutsideLoopScope + ":\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}