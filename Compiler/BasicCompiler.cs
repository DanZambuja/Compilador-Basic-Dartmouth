using System;
using BasicReader;
using Compiler.LexicalAnalysis;
using Compiler.AsciiCategorizer;

namespace Compiler
{
    public class BasicCompiler {
        private BasicFileReader reader;
        private Categorizer asciiCategorizer;
        private LexicalEventEngine lexical;

        public BasicCompiler() {
            this.reader = new BasicFileReader();
            this.asciiCategorizer = new Categorizer();
            this.lexical = new LexicalEventEngine();
            this.asciiCategorizer.NotifySymbolCategorization += this.lexical.ConsumeCategorizedSymbolEvent;
        }
        public void CompileBasicPrograms(string path) {
            string file = this.reader.ReadAllText(path) + ' ';
            foreach(char symbol in file) {
                this.asciiCategorizer.Categorize(symbol);
            }
        }
    }
}