﻿using System;
using System.Collections.Generic;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;

namespace Brainf_ckSharp.Models;

/// <summary>
/// A <see langword="class"/> that contains info on an executed script
/// </summary>
public sealed class InterpreterResult
{
    /// <summary>
    /// Creates a new <see cref="InterpreterResult"/> instance with the specified parameters
    /// </summary>
    /// <param name="sourceCode">The original source code for the interpreted script</param>
    /// <param name="exitCode">The exit code of the interpreter result</param>
    /// <param name="haltingInfo">The debug info for the current script, if available</param>
    /// <param name="machineState">The resulting memory state after running the script</param>
    /// <param name="functions">The sequence of functions that were defined when running the script</param>
    /// <param name="stdin">The stdin buffer used to run the script</param>
    /// <param name="stdout">The resulting stdout buffer after running the script</param>
    /// <param name="elapsedTime">The execution time for the interpreted script</param>
    /// <param name="totalOperations">The total numer of evaluated operators for the current result</param>
    internal InterpreterResult(
        string sourceCode,
        ExitCode exitCode,
        HaltedExecutionInfo? haltingInfo,
        IReadOnlyMachineState machineState,
        IReadOnlyList<FunctionDefinition> functions,
        string stdin,
        string stdout,
        TimeSpan elapsedTime,
        int totalOperations)
    {
        SourceCode = sourceCode;
        ExitCode = exitCode;
        HaltingInfo = haltingInfo;
        MachineState = machineState;
        Functions = functions;
        Stdin = stdin;
        Stdout = stdout;
        ElapsedTime = elapsedTime;
        TotalOperations = totalOperations;
    }

    /// <summary>
    /// Gets the original source code for the interpreted script
    /// </summary>
    public string SourceCode { get; }

    /// <summary>
    /// Gets the exit code of the interpreter result
    /// </summary>
    public ExitCode ExitCode { get; }

    /// <summary>
    /// Gets the debug info for the current script, if its exection was stopped
    /// </summary>
    public HaltedExecutionInfo? HaltingInfo { get; }

    /// <summary>
    /// Gets the resulting memory state after running the script
    /// </summary>
    public IReadOnlyMachineState MachineState { get; }

    /// <summary>
    /// Gets the sequence of functions that were defined when running the script
    /// </summary>
    public IReadOnlyList<FunctionDefinition> Functions { get; }

    /// <summary>
    /// Gets the stdin buffer used to run the script
    /// </summary>
    public string Stdin { get; }

    /// <summary>
    /// Gets the resulting stdout buffer after running the script
    /// </summary>
    public string Stdout { get; }

    /// <summary>
    /// Gets the execution time for the interpreted script
    /// </summary>
    public TimeSpan ElapsedTime { get; }

    /// <summary>
    /// Gets the total numer of evaluated operators for the current result
    /// </summary>
    public int TotalOperations { get; }
}
