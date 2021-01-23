using System;
using FileIO;
using Compiler.LexicalAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public class PrintCommand : ICommand
    {
        private int printStringCount = 0;

        private FileManager fileManager;

        public PrintCommand(FileManager fileManager) {
            this.fileManager = fileManager;
        }

        public void ConsumeToken(Token token) {
            if (token.Type == TokenType.STRING || token.Type == TokenType.QUOTED_STRING) {
                this.GeneratePrintString(token.Text);
            } else if (token.Type == TokenType.INT) {
                this.GeneratePrintInt(token.Text);
            } else if (token.Type == TokenType.ARRAY) {
                this.GeneratePrintVar(0);
            } else if (token.Type == TokenType.ARRAY_ELEMENT) {
                this.GeneratePrintVar(0);
            } else if (token.Type == TokenType.VAR) {
                this.GeneratePrintVar(0);
            } 
        }
        
        public void GeneratePrintString(string message) {
            this.printStringCount++;
            string instructions = string.Empty;

            instructions += "   ldr r0, =PRINT_" + this.printStringCount + "\n";
            instructions += "   bl print_uart0\n";

            this.fileManager.WriteInstructionsToFile(instructions);

            string memorySpaceForString = string.Empty;
            message = message.Insert(message.Length - 1, @"\012\000");

            memorySpaceForString += "PRINT_" + this.printStringCount + ": .ascii " + message + "\n";

            this.fileManager.WritePrintDataSection(memorySpaceForString);
        }

        private void GeneratePrintVar(int varIndex) {
            string instructions = string.Empty;

            // as variaveis definidas no programa sao armazenadas numa tabela
            // a partir do endereço do inicio da tabela mem
            // calcula-se o endereço em que a variavel foi armazenada
            // fazendo 4 * índice + mem, e desse endereço coloco o valor no registrador r0
            // depois chamo a função que printa o resultado

            instructions += "   adr r1, mem\n";
            instructions += "   ldr r2, =4\n";
            instructions += string.Format("   ldr r3, ={0}\n", varIndex);
            instructions += "   mul r4, r3, r2\n";
            instructions += "   add r5, r4, r1\n";
            instructions += "   ldr r0, [r5]\n";
            instructions += "   bl print_uart0\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }

        private void GeneratePrintInt(string number) {
            string instructions = string.Empty;

            instructions += "   ldr r0, =" + number + "\n";
            instructions += "   bl print_uart0\n";

            this.fileManager.WriteInstructionsToFile(instructions);
        }
    }
}