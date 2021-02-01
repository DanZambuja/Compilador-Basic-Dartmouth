using Compiler;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string basicProgramPath = "./Programs_in_BASIC/9.bas";
            string instructionsFile = "./Output/Instructions.s";
            string printDataFile = "./Output/PrintData.s";
            string varAndArrayTableFile = "./Output/VarAndArrayTable.s";
            string baseFile = "./InitialSetupForArmFile/Base.s";
            string beforePrintDataFile = "./InitialSetupForArmFile/BeforePrintData.s";
            string finalOutputFile = "./CompiledProgram/program.s";

            BasicCompiler compiler = new BasicCompiler(
                instructionsFile, 
                printDataFile, 
                varAndArrayTableFile,
                baseFile,
                beforePrintDataFile,
                finalOutputFile);

            compiler.CompileBasicPrograms(basicProgramPath);
        }
    }
}
