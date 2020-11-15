using System;

namespace Compiler
{
    public class SyntaxAnalizer {
        public SyntaxAnalizer() { 
        }

        public void Analyze(string file) {
            foreach(char element in file) {
                Console.WriteLine(element);
            }
        }
    }
}