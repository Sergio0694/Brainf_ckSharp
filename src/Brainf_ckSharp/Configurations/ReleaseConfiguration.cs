using System;
using System.Threading;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;

namespace Brainf_ckSharp.Configurations;

/// <summary>
/// A model for a RELEASE configuration being built
/// </summary>
public readonly ref partial struct ReleaseConfiguration
{
    /// <summary>
    /// The source code to parse and execute
    /// </summary>
    public required ReadOnlyMemory<char> Source { get; init; }

    /// <summary>
    /// The (optional) stdin buffer to use to run the script
    /// </summary>
    public ReadOnlyMemory<char> Stdin { get; init; }

    /// <summary>
    /// The (optional) initial machine state to use to execute the script
    /// </summary>
    public IReadOnlyMachineState? InitialState { get; init; }

    /// <summary>
    /// The (optional) memory size for the machine state to use
    /// </summary>
    public int? MemorySize { get; init; }

    /// <summary>
    /// The (optional) data type to use to run the script
    /// </summary>
    public DataType? DataType { get; init; }

    /// <summary>
    /// The setting to control the execution options to run the script
    /// </summary>
    public ExecutionOptions ExecutionOptions { get; init; }

    /// <summary>
    /// The token to cancel a long running execution
    /// </summary>
    public CancellationToken ExecutionToken { get; init; }
}
