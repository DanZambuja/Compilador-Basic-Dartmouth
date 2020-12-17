using System;
using System.IO;

namespace FileIO
{
    public class FileManager
    {
        private readonly string instructionFile;
        private readonly string printDataFile;
        private readonly string varAndArrayTable;
        private readonly string baseFile;
        private readonly string beforePrintDataFile;
        private readonly string finalOutputFile;
        public FileManager(
            string instructionFile, 
            string printDataFile, 
            string varAndArrayTable,
            string baseFile,
            string beforePrintDataFile,
            string finalOutputFile) 
        { 
            this.instructionFile = instructionFile;
            this.printDataFile = printDataFile;
            this.varAndArrayTable = varAndArrayTable;
            this.baseFile = baseFile;
            this.beforePrintDataFile = beforePrintDataFile;
            this.finalOutputFile = finalOutputFile;

        }
        
        // Estrutura o arquivo final para execução
        // base.s + Instructions.s + BeforePrintData.s + PrintData.s + VarAndArrayTable.s
        public void CompileOutputFile() {
            string baseFileData = this.ReadAllText(this.baseFile);
            string instructionFileData = this.ReadAllText(this.instructionFile);
            string beforePrintDataFileData = this.ReadAllText(this.beforePrintDataFile);
            string printDataFileData = this.ReadAllText(this.printDataFile);
            string varAndArrayTableData = this.ReadAllText(this.varAndArrayTable);

            this.WriteCompiledText(this.finalOutputFile, baseFileData);
            this.WriteCompiledText(this.finalOutputFile, instructionFileData);
            this.WriteCompiledText(this.finalOutputFile, beforePrintDataFileData);
            this.WriteCompiledText(this.finalOutputFile, printDataFileData);
            this.WriteCompiledText(this.finalOutputFile, varAndArrayTableData);
        }

        public void WriteInstructionsToFile(string instructions) {
            this.WriteCompiledText(this.instructionFile, instructions);
        }

        public void WritePrintDataSection(string printData) {
            this.WriteCompiledText(this.printDataFile, printData);
        }

        public void WriteVarAndDataTable(string data) {
            this.WriteCompiledText(this.varAndArrayTable, data);
        }

        private void WriteCompiledText(string output, string data) {
            File.AppendAllText(output, data);
        }
        
        public string[] ReadAllLines(string path) {
            string[] lines = null;

            if(File.Exists(path)) {
                lines = File.ReadAllLines(path);
            }

            return lines;
        }

        public string ReadAllText(string path) {
            string file = null;

            if(File.Exists(path)) {
                file = File.ReadAllText(path);
            }

            return file;
        }
    }
}
