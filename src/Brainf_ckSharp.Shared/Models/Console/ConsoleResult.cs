using Brainf_ckSharp.Shared.Models.Console.Interfaces;

namespace Brainf_ckSharp.Shared.Models.Console;

/// <summary>
/// A model for a console result with an output text
/// </summary>
public sealed class ConsoleResult : IConsoleEntry
{
    /// <summary>
    /// Creates a new <see cref="ConsoleResult"/> instaance with the specified parameters
    /// </summary>
    /// <param name="stdout">The stdout result for the current instance</param>
    public ConsoleResult(string stdout) => Stdout = stdout;

    /// <summary>
    /// Gets the stdout result for the current instance
    /// </summary>
    public string Stdout { get; }
}
