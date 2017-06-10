using System;
using System.Collections.Generic;
using Brainf_ck_sharp.MemoryState;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.ReturnTypes
{
    /// <summary>
    /// A class that contains all the info on an interpreted Brainf_ck script
    /// </summary>
    public sealed class InterpreterResult
    {
        #region Public APIs

        /// <summary>
        /// Gets the exit code for the interpreted script, with all the relevant flags
        /// </summary>
        public InterpreterExitCode ExitCode { get; }

        /// <summary>
        /// Checks whether or not the current <see cref="ExitCode"/> property contains a specific flag
        /// </summary>
        /// <param name="flag">The flag to check (it must have a single bit set)</param>
        [PublicAPI]
        [Pure]
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
        public IReadonlyTouringMachineState MachineState { get; }

        /// <summary>
        /// Gets the execution time for the interpreted script
        /// </summary>
        public TimeSpan ElapsedTime { get; }

        /// <summary>
        /// Gets the total numer of evaluated operators for the current result
        /// </summary>
        public uint TotalOperations { get; }

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
        public InterpreterExceptionInfo ExceptionInfo { get; }

        #endregion

        #region Internal methods

        /// <summary>
        /// Gets the position of the breakpoint that caused the script to half, if present
        /// </summary>
        [CanBeNull]
        internal uint? BreakpointPosition { get; }

        // Internal constructor
        internal InterpreterResult(InterpreterExitCode exitCode, [NotNull] TouringMachineState state, TimeSpan duration,
            [NotNull] String output, [NotNull] String code, uint operations, [CanBeNull] IReadOnlyList<String> stackTrace, uint? breakpoint)
        {
            ExitCode = exitCode;
            MachineState = state;
            ElapsedTime = duration;
            Output = output;
            SourceCode = code;
            TotalOperations = operations;
            if (stackTrace != null) ExceptionInfo = new InterpreterExceptionInfo(stackTrace, code);
            if (breakpoint != null) BreakpointPosition = breakpoint;
        }

        // Private constructor for the Clone method
        private InterpreterResult(InterpreterExitCode exitCode, [NotNull] TouringMachineState state, TimeSpan duration,
            [NotNull] String output, [NotNull] String code, uint operations, [CanBeNull] InterpreterExceptionInfo debugInfo, uint? breakpoint)
        {
            ExitCode = exitCode;
            MachineState = state;
            ElapsedTime = duration;
            Output = output;
            SourceCode = code;
            TotalOperations = operations;
            if (debugInfo != null) ExceptionInfo = debugInfo;
            if (breakpoint != null) BreakpointPosition = breakpoint;
        }

        /// <summary>
        /// Creates a copu of the current result
        /// </summary>
        /// <returns></returns>
        [Pure, NotNull]
        internal InterpreterResult Clone()
        {
            return new InterpreterResult(ExitCode, ((TouringMachineState)MachineState).Clone(), ElapsedTime, Output, SourceCode, TotalOperations, ExceptionInfo, BreakpointPosition);
        }

        #endregion
    }
}
