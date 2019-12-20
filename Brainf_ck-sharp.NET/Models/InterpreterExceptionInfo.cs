using System.Collections.Generic;

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A <see langword="class"/> that exposes info to debug a faulted Brainf*ck/PBrain script
    /// </summary>
    public sealed class InterpreterExceptionInfo
    {
        /// <summary>
        /// Gets the stack trace for the current instance
        /// </summary>
        public IReadOnlyList<string> StackTrace { get; }

        /// <summary>
        /// Gets the operator that generated the exception in the script
        /// </summary>
        public char FaultedOperator { get; }

        /// <summary>
        /// Gets the position of the operator that generated the error in the executed script
        /// </summary>
        /// <remarks>The error refers to the processed script, with all the comments removed</remarks>
        public int ErrorOffset { get; }

        /// <summary>
        /// Creates a new <see cref="InterpreterExceptionInfo"/> instance with the specified parameters
        /// </summary>
        /// <param name="stackTrace">The stack trace for the current instance</param>
        /// <param name="faultedOperator">The operator that generated the exception in the script</param>
        /// <param name="errorOffset">The position of the operator that generated the error in the executed script</param>
        internal InterpreterExceptionInfo(IReadOnlyList<string> stackTrace, char faultedOperator, int errorOffset)
        {
            StackTrace = stackTrace;
            FaultedOperator = faultedOperator;
            ErrorOffset = errorOffset;
        }
    }
}
