using System;
using System.Collections.Generic;
using System.Text;
using Brainf_ck_sharp.Exceptions;
using Branf_ck_sharp;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    public sealed class InterpreterResult
    {
        public InterpreterExitCode ExitCode { get; }

        [NotNull]
        public TouringMachineState MachineState { get; }

        public TimeSpan ElapsedTime { get; }

        [NotNull]
        public String Output { get; }

        [NotNull]
        public String SourceCode { get; }

        [CanBeNull]
        public InterpreterException Exception { get; }

        internal InterpreterResult()
        {
            
        }
    }
}
