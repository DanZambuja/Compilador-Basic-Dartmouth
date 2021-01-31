using FileIO;
using System;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using System.Collections.Generic;

namespace Compiler.ArmRoutineGenerators
{
    public enum ForMachineState
    {
        START,
        FOR,
        VAR,
        EQUALS,
        TO,
        STEP
    }

    public class ForStateMachine : ISubStateMachine {
        private Dictionary<ForStateTransition, ForMachineState> transitions;
        public ForMachineState CurrentState { get; private set; }
        private ForCommand command;
        private VariableTable variables;
        private Token loopedVariable;
        private ArithmeticStateMachine exp;

        public ForStateMachine(VariableTable variables, FileManager fileManager) {
            CurrentState = ForMachineState.START;
            transitions = new Dictionary<ForStateTransition, ForMachineState>
            {
                { new ForStateTransition(ForMachineState.START, TokenType.END), ForMachineState.START },
                { new ForStateTransition(ForMachineState.START, TokenType.FOR), ForMachineState.FOR },
                // guarda variavel a ser loopada
                { new ForStateTransition(ForMachineState.FOR, TokenType.VAR), ForMachineState.VAR }, 
                { new ForStateTransition(ForMachineState.FOR, TokenType.ARRAY), ForMachineState.VAR }, 
                // recebe token que indica a atribuicao do valor a seguir a variavel recebida
                { new ForStateTransition(ForMachineState.VAR, TokenType.EQUALS), ForMachineState.EQUALS },
                // calcula valor da expressao a ser usada como valor inicial -> atribui a variavel a ser loopada armazenada anteriormente (guarda na memoria)
                { new ForStateTransition(ForMachineState.EQUALS, TokenType.TO), ForMachineState.TO }, 
                // calcula valor da expressao a ser usada como valor final -> guarda na memoria esse valor para futuras comparacoes
                { new ForStateTransition(ForMachineState.TO, TokenType.STEP), ForMachineState.STEP },
                // calcula valor da expressao a ser usada como passo -> guarda na memoria esse valor para futuro uso na hora de executar o passo
                { new ForStateTransition(ForMachineState.STEP, TokenType.END), ForMachineState.START },
            };
            this.variables = variables;
            this.command = new ForCommand(variables, fileManager);
            this.exp = new ArithmeticStateMachine(variables, fileManager);
        }

        public ForMachineState GetNext(Token token)
        {
            ForMachineState nextState = this.CurrentState;
            ForStateTransition transition = new ForStateTransition(CurrentState, token.Type);

            if (this.CurrentState == ForMachineState.EQUALS && token.Type == TokenType.TO) {
                this.exp.Reset(); //valor inicial a ser atribuido na variavel esta em r0
                this.command.ReceivedLoopedVariable(this.loopedVariable);
            } else if (this.CurrentState == ForMachineState.TO && token.Type == TokenType.STEP) {
                this.exp.Reset(); //valor maximo para parada do loop esta em r0
                this.command.SaveMaxValueForLoop(this.loopedVariable);
            } else if (this.CurrentState == ForMachineState.STEP && token.Type == TokenType.END) {
                this.exp.Reset(); //valor de passo do loop esta em r0
                this.command.SaveStepValueForLoop(this.loopedVariable);
            } 

            if ((this.CurrentState == ForMachineState.EQUALS && token.Type != TokenType.TO  ) ||
                (this.CurrentState == ForMachineState.TO     && token.Type != TokenType.STEP) || 
                (this.CurrentState == ForMachineState.STEP   && token.Type != TokenType.END )) {
                this.exp.MoveToNextState(token);
            } else if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("For: Invalid transition: " + CurrentState + " -> " + nextState + "\n" + token.Text + " " + token.Type);

            if (this.CurrentState == ForMachineState.FOR && (token.Type == TokenType.VAR || token.Type == TokenType.ARRAY)) {
                this.loopedVariable = token;
            }

            Console.WriteLine("FOR: " + this.CurrentState + " -> " + nextState + ": " + token.Text);
            
            return nextState;
        }

        public void MoveToNextState(Token token)
        {
            ForMachineState nextState = GetNext(token);

            this.CurrentState = nextState;
        }

        class ForStateTransition {

            public ForMachineState CurrentState { get; private set; }
            public TokenType Token { get; private set; }

            public ForStateTransition(ForMachineState currentState, TokenType token)
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
                ForStateTransition other = obj as ForStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Token == other.Token;
            }
        }
    }
}