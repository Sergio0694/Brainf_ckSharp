using System;
using System.Collections.Generic;
using System.Text;

namespace Brainf_ck_sharp.Exceptions
{
    public sealed class InterpreterException : Exception
    {
        public ExceptionType Type { get; }

        public Stack<String> StackTrace { get; }
    }
}
