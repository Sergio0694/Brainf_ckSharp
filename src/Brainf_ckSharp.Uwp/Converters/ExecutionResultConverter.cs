using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;

#nullable enable

namespace Brainf_ckSharp.Uwp.Converters
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions for bindings to <see cref="Option{T}"/> of <see cref="InterpreterResult"/> values
    /// </summary>
    public static class ExecutionResultConverter
    {
        /// <summary>
        /// Checks whether a given result represents a successful run
        /// </summary>
        /// <param name="result">The input result to convert</param>
        /// <returns>A <see cref="bool"/> value for the input result and requested state to check</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmptyOrSuccessfulRun(Option<InterpreterResult>? result)
        {
            return
                result is null ||
                result.ValidationResult.IsEmptyScript ||
                result.Value?.ExitCode == ExitCode.Success;
        }

        /// <summary>
        /// Checks whether a given result represents a successful run
        /// </summary>
        /// <param name="result">The input result to convert</param>
        /// <returns>A <see cref="bool"/> value for the input result and requested state to check</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFailedRun(Option<InterpreterResult>? result)
        {
            return result?.Value?.ExitCode != ExitCode.Success;
        }
    }
}
