using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;

namespace Brainf_ckSharp.Configurations
{
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
        [Pure]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Option<InterpreterSession> TryRun()
        {
            Guard.MustBeNotNull(Source, nameof(Source));

            if (InitialState is TuringMachineState initialState)
            {
                Guard.MustBeNull(MemorySize, nameof(MemorySize));
                Guard.MustBeNull(OverflowMode, nameof(OverflowMode));

                initialState = (TuringMachineState)initialState.Clone();
            }
            else
            {
                Guard.MustBeNotNull(MemorySize, nameof(MemorySize));
                Guard.MustBeGreaterThanOrEqualTo(MemorySize!.Value, Specs.MinimumMemorySize, nameof(MemorySize));
                Guard.MustBeLessThanOrEqualTo(MemorySize!.Value, Specs.MaximumMemorySize, nameof(MemorySize));
                Guard.MustBeNotNull(OverflowMode, nameof(OverflowMode));

                initialState = new TuringMachineState(MemorySize.Value, OverflowMode!.Value);
            }

            return Brainf_ckInterpreter.Debug.TryCreateSession(
                Source!,
                Breakpoints.Span,
                Stdin ?? string.Empty,
                initialState,
                ExecutionToken,
                DebugToken);
        }
    }
}
