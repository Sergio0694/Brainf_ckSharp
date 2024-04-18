using Brainf_ckSharp.Shared.Models.Console.Interfaces;

namespace Brainf_ckSharp.Shared.Models.Console;

/// <summary>
/// A model for a console result with an output text
/// </summary>
public sealed class ConsoleResult : IConsoleEntry
{
    /// <summary>
    /// Gets the stdout result for the current instance
    /// </summary>
    public required string Stdout { get; init; }
}
