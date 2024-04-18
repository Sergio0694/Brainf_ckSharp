using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Uwp.Converters.Console;

#nullable enable

namespace Brainf_ckSharp.Uwp.Converters;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFailedRun(Option<InterpreterResult>? result)
    {
        return result?.Value?.ExitCode != ExitCode.Success;
    }

    /// <summary>
    /// Returns a representation of a given <see cref="Option{T}"/> of <see cref="InterpreterResult"/> run
    /// </summary>
    /// <param name="result">The input result to convert</param>
    /// <returns>A representation of the input run</returns>
    public static string? StdoutOrExitCodeDescription(Option<InterpreterResult>? result)
    {
        if (result is null)
        {
            return null;
        }

        // No result if the script is empty
        if (result.ValidationResult.IsEmptyScript)
        {
            return null;
        }

        // Show the syntax error, if present
        if (result.ValidationResult.IsError)
        {
            return SyntaxValidationResultConverter.Convert(result.ValidationResult);
        }

        // Show the exception message, if any
        if (result.Value!.ExitCode != ExitCode.Success)
        {
            return ExitCodeConverter.Convert(result.Value!.ExitCode);
        }

        return result.Value!.Stdout.Length > 0 ? result.Value!.Stdout : null;
    }
}
