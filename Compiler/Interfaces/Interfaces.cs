using Compiler.LexicalAnalysis;

namespace Compiler.ArmRoutineGenerators
{
    public interface ICommand
    {
        void ConsumeToken(Token token);
    }

    public interface ISubStateMachine {
        void MoveToNextState(Token token);
    }
}