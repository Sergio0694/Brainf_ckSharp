using Brainf_ckSharp.Enums;

namespace Brainf_ckSharp.Unit.Shared.Models;

/// <summary>
/// A model for a loaded script to execute
/// </summary>
public sealed class Script
{
    /// <summary>
    /// Creates a new <see cref="Script"/> instance with the specified parameters
    /// </summary>
    /// <param name="stdin">The stdin buffer to feed the script</param>
    /// <param name="stdout">The expected stdout buffer for the script</param>
    /// <param name="memorySize">The memory size to use to run the script</param>
    /// <param name="overflowMode">The overflow mode to use to run the script</param>
    /// <param name="source">The source code of the script</param>
    public Script(
        string stdin,
        string stdout,
        int memorySize,
        OverflowMode overflowMode,
        string source)
    {
        Stdin = stdin;
        Stdout = stdout;
        MemorySize = memorySize;
        OverflowMode = overflowMode;
        Source = source;
    }

    /// <summary>
    /// Gets the stdin buffer to feed the script
    /// </summary>
    public string Stdin { get; }

    /// <summary>
    /// Gets the expected stdout buffer for the script
    /// </summary>
    public string Stdout { get; }

    /// <summary>
    /// Gets the memory size to use to run the script
    /// </summary>
    public int MemorySize { get; }

    /// <summary>
    /// Gets the overflow mode to use to run the script
    /// </summary>
    public OverflowMode OverflowMode { get; }

    /// <summary>
    /// Gets the source code of the script
    /// </summary>
    public string Source { get; }
}
