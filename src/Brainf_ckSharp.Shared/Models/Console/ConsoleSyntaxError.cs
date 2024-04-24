using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Models.Console.Interfaces;

namespace Brainf_ckSharp.Shared.Models.Console;

/// <summary>
/// A model for a console syntax error being produced while parsing a script
/// </summary>
public sealed class ConsoleSyntaxError : IConsoleEntry
{
    /// <summary>
    /// Gets the exit code for the executed command
    /// </summary>
    public required SyntaxValidationResult Result { get; init; }
}
