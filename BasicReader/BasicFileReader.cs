using System;
using System.IO;

namespace BasicReader
{
    public class BasicFileReader
    {
        public BasicFileReader() {

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
