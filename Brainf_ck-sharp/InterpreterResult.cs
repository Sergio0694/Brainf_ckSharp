using System;
using System.Collections.Generic;
using Branf_ck_sharp;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    public sealed class InterpreterResult
    {
        public InterpreterExitCode ExitCode { get; }

        public bool HasFlag(InterpreterExitCode flag)
        {
            // Check the flags set
            bool found = false;
            int bits = (ushort)flag;
            for (int i = 0; i < 16; i++)
            {
                if ((bits & 1) == 1)
                {
                    if (found) throw new ArgumentException("The input value has more than a single flag set");
                    found = true;
                }
                bits = bits >> 1;
            }
            if (!found) throw new ArgumentException("The input value doesn't have a flag set");

            // Check whether or not the input flag is valid for this instance
            return (ExitCode & flag) == flag;
        }

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
