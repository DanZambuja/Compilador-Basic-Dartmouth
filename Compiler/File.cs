using System;
using System.IO;

namespace FileIO
{
    public class FileManager
    {
        private readonly string outputFile;
        public FileManager(string outputFile) 
        { 
            this.outputFile = outputFile;
        }

        public void WriteCompiledLine(string compiledText) {
            File.AppendAllText(this.outputFile, compiledText);
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
