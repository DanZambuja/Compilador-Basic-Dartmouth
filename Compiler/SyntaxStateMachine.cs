using System;
using System.Collections.Generic;
using FileIO;
using Compiler.LexicalAnalysis;
using Compiler.ArmRoutineGenerators;

namespace Compiler.SyntaxAnalysis
{
    public enum SyntaxMachineState
    {
        START,
        PRINT,
        LET,
        FOR,
        NEXT,
        DIM,
        DATA,
        READ,
        REMARK,
        GO,
        IF,
        GOSUB,
        RETURN,
        DEF,
        FINAL
    }

    public delegate void OutputCompiledToWrite(string armCommand);

    public class SyntaxStateMachine 
    {
        public event OutputCompiledToWrite NotifyOutputCompiledToWrite;
        private Dictionary<SyntaxStateTransition, SyntaxMachineState> transitions;
        public SyntaxMachineState CurrentState { get; private set; }
        private VariableTable variables;
        private SyntaxEngine engine;
        private SequenceIdLabelCommand sequenceId;
        private EndCommand endCommand;
        private ReturnCommand returnCommand;
        private FileManager fileManager;

        public SyntaxStateMachine(FileManager fileManager) { 
            CurrentState = SyntaxMachineState.START;
            transitions = new Dictionary<SyntaxStateTransition, SyntaxMachineState> 
            {
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.INT),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.RETURN),   SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.FINAL),    SyntaxMachineState.START },
                
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.PRINT),    SyntaxMachineState.PRINT },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.LET),      SyntaxMachineState.LET },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.FOR),      SyntaxMachineState.FOR },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.DIM),      SyntaxMachineState.DIM },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.REMARK),   SyntaxMachineState.REMARK },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.GO),       SyntaxMachineState.GO },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.GOTO),     SyntaxMachineState.GO },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.DATA),     SyntaxMachineState.DATA },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.READ),     SyntaxMachineState.READ },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.GOSUB),    SyntaxMachineState.GOSUB },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.IF),       SyntaxMachineState.IF },
                { new SyntaxStateTransition(SyntaxMachineState.START,   TokenType.NEXT),     SyntaxMachineState.NEXT },

                { new SyntaxStateTransition(SyntaxMachineState.PRINT,   TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.LET,     TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.FOR,     TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.DIM,     TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.REMARK,  TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.GO,      TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.DATA,    TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.READ,    TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.GOSUB,   TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.IF,      TokenType.END),      SyntaxMachineState.START },
                { new SyntaxStateTransition(SyntaxMachineState.NEXT,    TokenType.END),      SyntaxMachineState.START },
            };
            this.fileManager    = fileManager;
            this.variables      = new VariableTable();
            this.endCommand     = new EndCommand(fileManager);
            this.sequenceId     = new SequenceIdLabelCommand(this.variables, fileManager);
            this.returnCommand  = new ReturnCommand(this.variables, fileManager);
            this.engine         = new SyntaxEngine(this.variables, fileManager);
        }

        public void ConsumeIdentifiedTokenEvent(Token token) {
            if (token.Type == TokenType.ARRAY || token.Type == TokenType.ARRAY_ELEMENT) {
                Console.WriteLine("Syntax1: " + token.Text + " - " + token.IndexOrSize.ToString() + " - " + token.Type.ToString());
            } else {
                Console.WriteLine("Syntax2: " + token.Text + " - " + token.Type.ToString());
            }
            this.MoveNext(token);
        }

        protected virtual void OnOutputCompiled(string armCommand) {
            NotifyOutputCompiledToWrite?.Invoke(armCommand);
        }

        public SyntaxMachineState GetNext(Token token)
        {
            SyntaxMachineState nextState = this.CurrentState;
            SyntaxStateTransition transition = new SyntaxStateTransition(CurrentState, token.Type);

            if ((this.CurrentState == SyntaxMachineState.START || this.CurrentState != SyntaxMachineState.START && token.Type == TokenType.END) && 
                !transitions.TryGetValue(transition, out nextState))
                throw new Exception("Syntax: Invalid transition: " + CurrentState + " -> " +  nextState + "\nToken Text: " + token.Text + " Token Type: " + token.Type.ToString());

            if (this.CurrentState == SyntaxMachineState.START && token.Type == TokenType.INT)
                this.sequenceId.ConsumeToken(token);
            else if (this.CurrentState == SyntaxMachineState.START && token.Type == TokenType.FINAL)
                this.endCommand.EndInstruction();
            else if (this.CurrentState == SyntaxMachineState.START && token.Type == TokenType.RETURN)
                this.returnCommand.ReturnInstructions();
            else
                this.engine.ConsumeToken(this.CurrentState, token, nextState);
            
            
            Console.WriteLine("Syntax: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public SyntaxMachineState MoveNext(Token token)
        {
            SyntaxMachineState nextState = GetNext(token);
            
            this.CurrentState = nextState;
            return CurrentState;
        }

        public void End() {
            FinalizationCommand final = new FinalizationCommand(this.fileManager);
            final.AllocateMemorySpaceForVariables(this.variables.variableCounter);
        }

        class SyntaxStateTransition {

            public SyntaxMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public SyntaxStateTransition(SyntaxMachineState currentState, TokenType token)
            {
                this.CurrentState = currentState;
                this.Token = token;
            }
            
            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Token.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                SyntaxStateTransition other = obj as SyntaxStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }

        }

        class SyntaxEngine {
            private Dictionary<SyntaxMachineState, ISubStateMachine> subStateMachines;
            public SyntaxEngine(VariableTable variables, FileManager fileManager) {
                this.subStateMachines = new Dictionary<SyntaxMachineState, ISubStateMachine> 
                {
                    { SyntaxMachineState.REMARK,    new RemarkStateMachine(           fileManager) },
                    { SyntaxMachineState.GO,        new GoStateMachine    (           fileManager) },
                    { SyntaxMachineState.PRINT,     new PrintStateMachine (variables, fileManager) },
                    { SyntaxMachineState.LET,       new LetStateMachine   (variables, fileManager) },
                    { SyntaxMachineState.DIM,       new DimStateMachine   (variables, fileManager) },
                    { SyntaxMachineState.DATA,      new DataStateMachine  (variables, fileManager) },
                    { SyntaxMachineState.READ,      new ReadStateMachine  (variables, fileManager) },
                    { SyntaxMachineState.GOSUB,     new GoSubStateMachine (variables, fileManager) },
                    { SyntaxMachineState.IF,        new IfStateMachine    (variables, fileManager) },
                    { SyntaxMachineState.FOR,       new ForStateMachine   (variables, fileManager) },
                    { SyntaxMachineState.NEXT,      new NextStateMachine  (variables, fileManager) },
                };
            }

            public void ConsumeToken(SyntaxMachineState currentState, Token token, SyntaxMachineState nextState) {
                if (currentState == SyntaxMachineState.START && nextState != SyntaxMachineState.START) {
                    this.subStateMachines[nextState].MoveToNextState(token);
                } else if (currentState != SyntaxMachineState.START) {
                    this.subStateMachines[currentState].MoveToNextState(token);
                }
            }
        }
    }
    public class VariableTable 
    {
        public int currentProgramLine = 0; // linha atual de execucao
        public int variableCounter = 0; // contador de quantas variaveis foram declaradas para separar espaço suficiente na memória
        public int lastGoSubCalled = 0; // indica a linha de retorno para o ultimo GoSub chamado
        public Dictionary<string, int> variableToIndex = new Dictionary<string, int>(); // mapa de indices das variaveis declaradas
        public Dictionary<string, int> variableToLoopStart = new Dictionary<string, int>(); // mapa do inicio (linha) do loop correspondente a cada variavel
        public Queue<Token> readVariables = new Queue<Token>(); // fila para variaveis que receberam comando read, e terão dados inseridos via comando data
        public void ResetReadVariables() {
            this.readVariables.Clear(); // limpa fila de variaveis caso dados inseridos nao forem suficientes para completar as variaveis listadas em read
        }
    }
}