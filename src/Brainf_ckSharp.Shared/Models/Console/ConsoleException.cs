﻿using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Models.Console.Interfaces;

namespace Brainf_ckSharp.Shared.Models.Console;

/// <summary>
/// A model for a console exception being thrown by an executed command
/// </summary>
public sealed class ConsoleException : IConsoleEntry
{
    /// <summary>
    /// Creates a new <see cref="ConsoleException"/> instance with the specified parameters
    /// </summary>
    /// <param name="exitCode">The exit code for the executed command</param>
    /// <param name="haltingInfo">The <see cref="HaltedExecutionInfo"/> instance for the current exception</param>
    public ConsoleException(ExitCode exitCode, HaltedExecutionInfo haltingInfo)
    {
        ExitCode = exitCode;
        HaltingInfo = haltingInfo;
    }

    /// <summary>
    /// Gets the exit code for the executed command
    /// </summary>
    public ExitCode ExitCode { get; }

    /// <summary>
    /// Gets the <see cref="HaltedExecutionInfo"/> instance for the current exception
    /// </summary>
    public HaltedExecutionInfo HaltingInfo { get; }
}
