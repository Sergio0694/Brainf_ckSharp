using System;
using System.Threading;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;

namespace Brainf_ckSharp.Configurations;

/// <summary>
/// A model for a DEBUG configuration being built
/// </summary>
public readonly ref partial struct DebugConfiguration
{
    /// <inheritdoc cref="ReleaseConfiguration.Source"/>
    public required ReadOnlyMemory<char> Source { get; init; }

    /// <inheritdoc cref="ReleaseConfiguration.Stdin"/>
    public ReadOnlyMemory<char> Stdin { get; init; }

    /// <inheritdoc cref="ReleaseConfiguration.InitialState"/>
    public IReadOnlyMachineState? InitialState { get; init; }

    /// <inheritdoc cref="ReleaseConfiguration.MemorySize"/>
    public int? MemorySize { get; init; }

    /// <inheritdoc cref="ReleaseConfiguration.DataType"/>
    public DataType? DataType { get; init; }

    /// <inheritdoc cref="ReleaseConfiguration.ExecutionOptions"/>
    public ExecutionOptions ExecutionOptions { get; init; }

    /// <summary>
    /// The sequence of indices for the breakpoints to apply to the script
    /// </summary>
    public ReadOnlyMemory<int> Breakpoints { get; init; }

    /// <inheritdoc cref="ReleaseConfiguration.ExecutionToken"/>
    public CancellationToken ExecutionToken { get; init; }

    /// <summary>
    /// The token to cancel the monitoring of breakpoints
    /// </summary>
    public CancellationToken DebugToken { get; init; }
}
