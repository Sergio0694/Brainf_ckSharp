using System;
using System.Collections.Generic;
using System.Text;
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
        public Stack<String> StackTrace { get; }

        internal InterpreterResult(InterpreterExitCode exitCode, [NotNull] TouringMachineState state, TimeSpan duration,
            [NotNull] String output, [NotNull] String code, [CanBeNull] Stack<String> stackTrace)
        {
            ExitCode = exitCode;
            MachineState = state;
            ElapsedTime = duration;
            Output = output;
            SourceCode = code;
            StackTrace = stackTrace;
        }
    }
}
