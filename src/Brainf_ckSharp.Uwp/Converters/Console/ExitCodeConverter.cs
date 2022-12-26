using System;
using System.Diagnostics;
using Brainf_ckSharp.Enums;
using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Uwp;

namespace Brainf_ckSharp.Uwp.Converters.Console;

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
    public static string Convert(ExitCode code)
    {
        Debug.Assert(code.HasFlag(ExitCode.Failure));

        Span<ExitCode> span = stackalloc[]
        {
            ExitCode.ThresholdExceeded,
            ExitCode.UpperBoundExceeded,
            ExitCode.LowerBoundExceeded,
            ExitCode.NegativeValue,
            ExitCode.MaxValueExceeded,
            ExitCode.StdinBufferExhausted,
            ExitCode.StdoutBufferLimitExceeded,
            ExitCode.UndefinedFunctionCalled,
            ExitCode.DuplicateFunctionDefinition,
            ExitCode.StackLimitExceeded,
        };

        foreach (ExitCode entry in span)
            if (code.HasFlag(entry))
                return $"{nameof(ExitCode)}/{entry}".GetLocalized();

        return ThrowHelper.ThrowArgumentException<string>(nameof(code), "Invalid exit code");
    }
}
