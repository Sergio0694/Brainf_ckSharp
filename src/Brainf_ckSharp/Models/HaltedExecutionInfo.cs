using System.Collections.Generic;

namespace Brainf_ckSharp.Models
{
    /// <summary>
    /// A <see langword="class"/> that exposes info to debug a halted Brainf*ck/PBrain script
    /// </summary>
    public sealed class HaltedExecutionInfo
    {
        /// <summary>
        /// Creates a new <see cref="HaltedExecutionInfo"/> instance with the specified parameters
        /// </summary>
        /// <param name="stackTrace">The stack trace for the current instance</param>
        /// <param name="haltingOperator">The operator that triggered the halting in the script</param>
        /// <param name="haltingOffset">The position of the operator that triggered the halting in the executed script</param>
        internal HaltedExecutionInfo(IReadOnlyList<string> stackTrace, char haltingOperator, int haltingOffset)
        {
            StackTrace = stackTrace;
            HaltingOperator = haltingOperator;
            HaltingOffset = haltingOffset;
        }

        /// <summary>
        /// Gets the stack trace for the current instance
        /// </summary>
        public IReadOnlyList<string> StackTrace { get; }

        /// <summary>
        /// Gets the operator that triggered the halting in the script
        /// </summary>
        public char HaltingOperator { get; }

        /// <summary>
        /// Gets the position of the operator that triggered the halting in the executed script
        /// </summary>
        /// <remarks>The offset refers to the processed script, with all the comments removed</remarks>
        public int HaltingOffset { get; }
    }
}
