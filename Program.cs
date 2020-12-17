using Compiler;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string basicProgramPath = "./Programs_in_BASIC/3.bas";
            string instructionsFile = "./Output/Instructions.s";
            string printDataFile = "./Output/PrintData.s";
            string varAndArrayTableFile = "./Output/VarAndArrayTable.s";
            string baseFile = "./InitialSetupForArmFile/Base.s";
            string beforePrintDataFile = "./InitialSetupForArmFile/BeforePrintData.s";
            string finalOutputFile = "./CompiledProgram/program.s";
            // string setupFilePath = "./InitialSetupForArmFile/AssemblySetupForBarebones.s";
            BasicCompiler compiler = new BasicCompiler(
                instructionsFile, 
                printDataFile, 
                varAndArrayTableFile,
                baseFile,
                beforePrintDataFile,
                finalOutputFile);
            // compiler.InitialSetupForBarebonesArmEnvironment(setupFilePath);
            compiler.CompileBasicPrograms(basicProgramPath);
        }
    }
}
