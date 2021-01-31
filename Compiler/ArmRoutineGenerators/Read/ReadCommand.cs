using System;
using Compiler.LexicalAnalysis;
using Compiler.SyntaxAnalysis;
using FileIO;

namespace Compiler.ArmRoutineGenerators
{
    public class ReadCommand : ICommand
    {
        private readonly FileManager fileManager;
        private readonly VariableTable variables;
        public ReadCommand(VariableTable variables, FileManager fileManager) {
            this.fileManager = fileManager;
            this.variables = variables;
        }
        public void ConsumeToken(Token token) {
            if (token.Type == TokenType.VAR || token.Type == TokenType.ARRAY) {
                if (!this.variables.variableToIndex.ContainsKey(token.Text) && token.Type == TokenType.VAR) {
                    this.variables.variableToIndex[token.Text] = this.variables.variableCounter++;
                } else if (!this.variables.variableToIndex.ContainsKey(token.Text)) {
                    throw new Exception("Array has to be defined with DIM before being attributed through READ");
                }
            } else {
                throw new Exception("Unexpected type on READ");
            }

            this.variables.readVariables.Enqueue(token);
        }
    }
}