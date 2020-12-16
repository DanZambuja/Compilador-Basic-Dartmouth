using Compiler;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string basicProgramPath = "./Programs_in_BASIC/3.bas";
            string outputFolderPath = "./Output/output.s";
            // string setupFilePath = "./InitialSetupForArmFile/AssemblySetupForBarebones.s";
            BasicCompiler compiler = new BasicCompiler(outputFolderPath);
            // compiler.InitialSetupForBarebonesArmEnvironment(setupFilePath);
            compiler.CompileBasicPrograms(basicProgramPath);
        }
    }
}
