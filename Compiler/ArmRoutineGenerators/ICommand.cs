using Compiler.LexicalAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public interface ICommand
    {
        void ConsumeToken(Token token);
        string GenerateArmInstructions();
        void Clear();
    }
}