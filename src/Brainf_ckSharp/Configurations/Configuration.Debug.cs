using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using CommunityToolkit.Diagnostics;

namespace Brainf_ckSharp.Configurations;

/// <summary>
/// A model for a DEBUG configuration being built
/// </summary>
public readonly ref partial struct DebugConfiguration
{
    /// <summary>
    /// The sequence of indices for the breakpoints to apply to the script
    /// </summary>
    public readonly ReadOnlyMemory<int> Breakpoints;

    /// <summary>
    /// The token to cancel the monitoring of breakpoints
    /// </summary>
    public readonly CancellationToken DebugToken;

    /// <summary>
    /// Runs the current Brainf*ck/PBrain configuration
    /// </summary>
    /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Option<InterpreterSession> TryRun()
    {
        Guard.IsNotNull(this.Source);

        if (this.InitialState is TuringMachineState initialState)
        {
            Guard.IsNull(this.MemorySize);
            Guard.IsNull(this.OverflowMode);

            initialState = (TuringMachineState)initialState.Clone();
        }
        else
        {
            int size = this.MemorySize ?? Specs.DefaultMemorySize;

            Guard.IsBetweenOrEqualTo(size, Specs.MinimumMemorySize, Specs.MaximumMemorySize, nameof(this.MemorySize));

            initialState = new TuringMachineState(size, this.OverflowMode ?? Specs.DefaultOverflowMode);
        }

        return Brainf_ckInterpreter.Debug.TryCreateSession(
            this.Source.Value.Span,
            this.Breakpoints.Span,
            this.Stdin.GetValueOrDefault(),
            initialState,
            this.ExecutionToken,
            this.DebugToken);
    }
}
