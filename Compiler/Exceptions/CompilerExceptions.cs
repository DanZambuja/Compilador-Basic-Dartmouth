using System;

namespace Compiler.Exceptions
{
    public class MismatchedBracesException : Exception
    {
        public MismatchedBracesException() { }
        public MismatchedBracesException(string message): base(message) { }
        public MismatchedBracesException(string message, Exception inner): base(message, inner) { }
    }
}