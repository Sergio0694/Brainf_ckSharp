using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.ReturnTypes
{
    /// <summary>
    /// Contains additional info to debug a faulted Brainf_ck script
    /// </summary>
    public sealed class InterpreterDebugInfo
    {
        /// <summary>
        /// Gets the stack trace for the exception thrown when running the script
        /// </summary>
        [NotNull]
        public IReadOnlyList<String> StackTrace { get; }

        /// <summary>
        /// Gets the operator that generated the exception in the script
        /// </summary>
        public char FaultedOperator { get; }

        /// <summary>
        /// Gets the position of the operator that generated the error inside the original source code (0-based index)
        /// </summary>
        public int ErrorPosition { get; }

        // Internal constructor
        internal InterpreterDebugInfo([NotNull] IReadOnlyList<String> stackTrace, String source)
        {
            StackTrace = stackTrace;
            ErrorPosition = stackTrace.Aggregate(0, (s, v) => s + v.Length) - 1;
            FaultedOperator = source[ErrorPosition];
        }
    }
}
