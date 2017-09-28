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

        // The underlying machine state used by the interpreter
        private readonly TouringMachineState _MachineState;

        /// <summary>
        /// Gets the resulting memory state after running the original script
        /// </summary>
        [NotNull]
        public IReadonlyTouringMachineState MachineState => _MachineState;

        /// <summary>
        /// Gets a list of the defined functions in the script
        /// </summary>
        [NotNull]
        public IReadOnlyList<FunctionDefinition> Functions { get; }

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
        internal InterpreterResult(
            InterpreterExitCode exitCode,
            [NotNull] TouringMachineState state, TimeSpan duration,
            [NotNull] String output, [NotNull] String code, uint operations, 
            [CanBeNull] InterpreterExceptionInfo info, uint? breakpoint,
            [NotNull] IReadOnlyList<FunctionDefinition> functions)
        {
            ExitCode = exitCode;
            _MachineState = state;
            ElapsedTime = duration;
            Output = output;
            SourceCode = code;
            TotalOperations = operations;
            if (info != null) ExceptionInfo = info;
            if (breakpoint != null) BreakpointPosition = breakpoint;
            Functions = functions;
        }

        // Internal failure constructor
        internal InterpreterResult(InterpreterExitCode result, [NotNull] TouringMachineState state, [NotNull] String code)
        {
            ExitCode = result;
            _MachineState = state;
            SourceCode = code;
            Output = String.Empty;
            ElapsedTime = TimeSpan.Zero;
            Functions = new FunctionDefinition[0];
        }

        /// <summary>
        /// Creates a copu of the current result
        /// </summary>
        [Pure, NotNull]
        internal InterpreterResult Clone()
        {
            return new InterpreterResult(ExitCode, _MachineState.Clone(), ElapsedTime, Output, SourceCode, TotalOperations, ExceptionInfo, BreakpointPosition, Functions);
        }

        #endregion
    }
}
