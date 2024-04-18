using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Models.Console.Interfaces;

namespace Brainf_ckSharp.Shared.Models.Console;

/// <summary>
/// A model for a console exception being thrown by an executed command
/// </summary>
public sealed class ConsoleException : IConsoleEntry
{
    /// <summary>
    /// Gets the exit code for the executed command
    /// </summary>
    public required ExitCode ExitCode { get; init; }

    /// <summary>
    /// Gets the <see cref="HaltedExecutionInfo"/> instance for the current exception
    /// </summary>
    public required HaltedExecutionInfo HaltingInfo { get; init; }
}
