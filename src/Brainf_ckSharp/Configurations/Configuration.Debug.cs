using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Microsoft.Toolkit.Diagnostics;

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
            Guard.IsNotNull(Source, nameof(Source));

            if (InitialState is TuringMachineState initialState)
            {
                Guard.IsNull(MemorySize, nameof(MemorySize));
                Guard.IsNull(OverflowMode, nameof(OverflowMode));

                initialState = (TuringMachineState)initialState.Clone();
            }
            else
            {
                int size = MemorySize ?? Specs.DefaultMemorySize;

                Guard.IsBetweenOrEqualTo(size, Specs.MinimumMemorySize, Specs.MaximumMemorySize, nameof(MemorySize));

                initialState = new TuringMachineState(size, OverflowMode ?? Specs.DefaultOverflowMode);
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
