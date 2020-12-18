using FileIO;
using Compiler.AsciiCategorizer;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;

namespace Compiler
{
    public class BasicCompiler {
        private FileManager fileManager;
        private Categorizer asciiCategorizer;
        private LexicalStateMachine lexical;
        private SyntaxStateMachine syntax;
        public BasicCompiler(
            string instructionsFile, 
            string printDataFile, 
            string varAndArrayFile,
            string baseFile,
            string beforePrintDataFile,
            string finalOutputFile) {

            this.fileManager = new FileManager(
                instructionsFile, 
                printDataFile, 
                varAndArrayFile,
                baseFile,
                beforePrintDataFile,
                finalOutputFile);

            this.asciiCategorizer = new Categorizer();
            this.lexical = new LexicalStateMachine();
            this.syntax = new SyntaxStateMachine(this.fileManager);
            this.asciiCategorizer.NotifySymbolCategorization += this.lexical.ConsumeCategorizedSymbolEvent;
            this.lexical.NotifyTokenIdentified += this.syntax.ConsumeIdentifiedTokenEvent;
        }
        
        public void CompileBasicPrograms(string path) {
            this.fileManager.ClearPreviouslyCompiledOutputFiles();
            string file = this.fileManager.ReadAllText(path) + ' ';
            foreach(char symbol in file) {
                this.asciiCategorizer.Categorize(symbol);
            }
            this.lexical.End();
            this.syntax.End();
            this.fileManager.CompileOutputFile();
        }
    }
}