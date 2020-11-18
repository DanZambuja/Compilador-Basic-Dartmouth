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
        public BasicCompiler(string outputPath) {
            this.fileManager = new FileManager(outputPath);
            this.asciiCategorizer = new Categorizer();
            this.lexical = new LexicalStateMachine();
            this.syntax = new SyntaxStateMachine();
            this.asciiCategorizer.NotifySymbolCategorization += this.lexical.ConsumeCategorizedSymbolEvent;
            this.lexical.NotifyTokenIdentified += this.syntax.ConsumeIdentifiedTokenEvent;
        }
        public void CompileBasicPrograms(string path) {
            string file = this.fileManager.ReadAllText(path) + ' ';
            foreach(char symbol in file) {
                this.asciiCategorizer.Categorize(symbol);
            }
        }
    }
}