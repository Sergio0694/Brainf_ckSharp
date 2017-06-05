using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.ReturnTypes
{
    /// <summary>
    /// A class that contains all the info on an interpreted Brainf_ck script
    /// </summary>
    public sealed class InterpreterResult
    {
        /// <summary>
        /// Gets the exit code for the interpreted script, with all the relevant flags
        /// </summary>
        public InterpreterExitCode ExitCode { get; }

        /// <summary>
        /// Checks whether or not the current <see cref="ExitCode"/> property contains a specific flag
        /// </summary>
        /// <param name="flag">The flag to check (it must have a single bit set)</param>
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

        /// <summary>
        /// Gets the resulting memory state after running the original script
        /// </summary>
        [NotNull]
        public TouringMachineState MachineState { get; }

        /// <summary>
        /// Gets the execution time for the interpreted script
        /// </summary>
        public TimeSpan ElapsedTime { get; }

        /// <summary>
        /// Gets the output produced by the script
        /// </summary>
        [NotNull]
        public String Output { get; }

        /// <summary>
        /// Gets the original raw source code for the interpreted script
        /// </summary>
        [NotNull]
        public String SourceCode { get; }

        /// <summary>
        /// If the script isn't executed successfully, gets all the relevant debug info
        /// </summary>
        [CanBeNull]
        public InterpreterDebugInfo DebugInfo { get; }

        /// <summary>
        /// Gets the position of the breakpoint that caused the script to half, if present
        /// </summary>
        [CanBeNull]
        internal int? BreakpointPosition { get; }

        // Internal constructor
        internal InterpreterResult(InterpreterExitCode exitCode, [NotNull] TouringMachineState state, TimeSpan duration,
            [NotNull] String output, [NotNull] String code, [CanBeNull] IReadOnlyList<String> stackTrace, int? breakpoint)
        {
            ExitCode = exitCode;
            MachineState = state;
            ElapsedTime = duration;
            Output = output;
            SourceCode = code;
            if (stackTrace != null) DebugInfo = new InterpreterDebugInfo(stackTrace, code);
            if (breakpoint != null) BreakpointPosition = breakpoint;
        }

        // Private constructor for the Clone method
        private InterpreterResult(InterpreterExitCode exitCode, [NotNull] TouringMachineState state, TimeSpan duration,
            [NotNull] String output, [NotNull] String code, [CanBeNull] InterpreterDebugInfo debugInfo, int? breakpoint)
        {
            ExitCode = exitCode;
            MachineState = state;
            ElapsedTime = duration;
            Output = output;
            SourceCode = code;
            if (debugInfo != null) DebugInfo = debugInfo;
            if (breakpoint != null) BreakpointPosition = breakpoint;
        }

        internal InterpreterResult Clone()
        {
            return new InterpreterResult(ExitCode, MachineState.Clone(), ElapsedTime,
                Output, SourceCode, DebugInfo, BreakpointPosition);
        }
    }
}
