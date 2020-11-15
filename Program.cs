using System;
using Compiler;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string basicProgramPath = "./Programs_in_BASIC/1.bas";
            BasicCompiler compiler = new BasicCompiler();
            compiler.CompileBasicPrograms(basicProgramPath);
        }
    }
}
