using System;
using BasicReader;

namespace Compiler
{
    public class BasicCompiler {

        private BasicFileReader reader;

        public BasicCompiler() {
            this.reader = new BasicFileReader();
        }

        public void CompileBasicPrograms(string path) {
            string[] lines = this.reader.ReadAllLines(path);
            foreach(string line in lines) {
                this.ProcessLineRead(line);
                Console.Write('\n');
            }
        }

        private void ProcessLineRead(string line) {
            string[] atoms = line.Split(' ');
            foreach(string atom in atoms) {
                Console.Write(atom);
                Console.Write(' ');
            }
        }
    }
}