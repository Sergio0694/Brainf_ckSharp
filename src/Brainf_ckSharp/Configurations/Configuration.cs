using System;
using System.Runtime.InteropServices;
using System.Threading;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;

namespace Brainf_ckSharp.Configurations;

/// <summary>
/// A model for a DEBUG configuration being built
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly ref partial struct DebugConfiguration
{
    /// <summary>
    /// The source code to parse and execute
    /// </summary>
    public readonly ReadOnlyMemory<char>? Source;

    /// <summary>
    /// The (optional) stdin buffer to use to run the script
    /// </summary>
    public readonly ReadOnlyMemory<char>? Stdin;

    /// <summary>
    /// The (optional) initial machine state to use to execute the script
    /// </summary>
    public readonly IReadOnlyMachineState? InitialState;

    /// <summary>
    /// The (optional) memory size for the machine state to use
    /// </summary>
    public readonly int? MemorySize;

    /// <summary>
    /// The (optional) data type to use to run the script
    /// </summary>
    public readonly DataType? DataType;

    /// <summary>
    /// The setting to control the execution options to run the script
    /// </summary>
    public readonly ExecutionOptions ExecutionOptions;

    /// <summary>
    /// The token to cancel a long running execution
    /// </summary>
    public readonly CancellationToken ExecutionToken;
}

/// <summary>
/// A model for a RELEASE configuration being built
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly ref partial struct ReleaseConfiguration
{
    /// <summary>
    /// The source code to parse and execute
    /// </summary>
    public readonly ReadOnlyMemory<char>? Source;

    /// <summary>
    /// The (optional) stdin buffer to use to run the script
    /// </summary>
    public readonly ReadOnlyMemory<char>? Stdin;

    /// <summary>
    /// The (optional) initial machine state to use to execute the script
    /// </summary>
    public readonly IReadOnlyMachineState? InitialState;

    /// <summary>
    /// The (optional) memory size for the machine state to use
    /// </summary>
    public readonly int? MemorySize;

    /// <summary>
    /// The (optional) data type to use to run the script
    /// </summary>
    public readonly DataType? DataType;

    /// <summary>
    /// The setting to control the execution options to run the script
    /// </summary>
    public readonly ExecutionOptions ExecutionOptions;

    /// <summary>
    /// The token to cancel a long running execution
    /// </summary>
    public readonly CancellationToken ExecutionToken;
}
