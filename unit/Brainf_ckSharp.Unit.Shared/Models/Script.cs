using Brainf_ckSharp.Enums;

namespace Brainf_ckSharp.Unit.Shared.Models;

/// <summary>
/// A model for a loaded script to execute
/// </summary>
public sealed class Script
{
    /// <summary>
    /// Gets the stdin buffer to feed the script
    /// </summary>
    public required string Stdin { get; init; }

    /// <summary>
    /// Gets the expected stdout buffer for the script
    /// </summary>
    public required string Stdout { get; init; }

    /// <summary>
    /// Gets the memory size to use to run the script
    /// </summary>
    public required int MemorySize { get; init; }

    /// <summary>
    /// Gets the data type to use to run the script
    /// </summary>
    public required DataType DataType { get; init; }

    /// <summary>
    /// Gets the additional execution options to run the script
    /// </summary>
    public required ExecutionOptions ExecutionOptions { get; init; }

    /// <summary>
    /// Gets the source code of the script
    /// </summary>
    public required string Source { get; init; }
}
