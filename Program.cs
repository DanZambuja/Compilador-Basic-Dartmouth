using Compiler;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string basicProgramPath = "./Programs_in_BASIC/1.bas";
            string outputFolderPath = "./";
            BasicCompiler compiler = new BasicCompiler(outputFolderPath);
            compiler.CompileBasicPrograms(basicProgramPath);
        }
    }
}
