using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Enums;

namespace Brainf_ckSharp.Uwp.Converters.Console
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions displaying runtime errors
    /// </summary>
    public static class ExitCodeConverter
    {
        /// <summary>
        /// Converts a given <see cref="ExitCode"/> instance to its representation
        /// </summary>
        /// <param name="code">The input <see cref="ExitCode"/> instance to format</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="ExitCode"/> value</returns>
        [Pure]
        public static string Convert(ExitCode code)
        {
            Debug.Assert(code.HasFlag(ExitCode.Failure));

            if (code.HasFlag(ExitCode.ThresholdExceeded)) return "Threshold exceeded";
            if (code.HasFlag(ExitCode.UpperBoundExceeded)) return "Upper bound exceeded";
            if (code.HasFlag(ExitCode.LowerBoundExceeded)) return "Lower exceeded";
            if (code.HasFlag(ExitCode.NegativeValue)) return "Negative value";
            if (code.HasFlag(ExitCode.MaxValueExceeded)) return "Maximum value exceeded";
            if (code.HasFlag(ExitCode.StdinBufferExhausted)) return "Stdin buffer exhausted";
            if (code.HasFlag(ExitCode.StdoutBufferLimitExceeded)) return "Stdout buffer limit exceeded";
            if (code.HasFlag(ExitCode.UndefinedFunctionCalled)) return "Undefined function called";
            if (code.HasFlag(ExitCode.DuplicateFunctionDefinition)) return "Duplicate function definition";
            if (code.HasFlag(ExitCode.StackLimitExceeded)) return "Stack limit exceeded";

            throw new ArgumentException($"Invalid exit code: {code}", nameof(code));
        }
    }
}
