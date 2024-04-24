using System.Collections.Generic;

namespace Brainf_ckSharp.Shared.Constants;

/// <summary>
/// A collection of code snippets to use
/// </summary>
public static class CodeSnippets
{
    /// <summary>
    /// Resets the current cell
    /// </summary>
    public const string ResetCell = "[-]";

    /// <summary>
    /// Duplicates the current value in the following cell
    /// </summary>
    public const string DuplicateValue = "[>+>+<<-]>>[<<+>>-]<<";

    /// <summary>
    /// Runs a block if the current cell is zero
    /// </summary>
    public const string IfZeroThen = ">+<[>-]>[->[-]]<<";

    /// <summary>
    /// Runs a block if the current cell is greater than zero, otherwise another block
    /// </summary>
    public const string IfGreaterThanZeroThenElse = ">+<[>[-]]>[->[-]]<<";

    /// <summary>
    /// Gets the collection of available code snippets
    /// </summary>
    public static IReadOnlyList<string> All { get; } =
    [
        ResetCell,
        DuplicateValue,
        IfZeroThen,
        IfGreaterThanZeroThenElse
    ];
}
