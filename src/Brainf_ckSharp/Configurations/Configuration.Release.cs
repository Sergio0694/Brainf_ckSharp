using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Opcodes;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace Brainf_ckSharp.Configurations
{
    /// <summary>
    /// A model for a RELEASE configuration being built
    /// </summary>
    public readonly ref partial struct ReleaseConfiguration
    {
        /// <summary>
        /// Runs the current Brainf*ck/PBrain configuration
        /// </summary>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Option<InterpreterResult> TryRun()
        {
            Guard.MustBeNotNull(Source, nameof(Source));

            using MemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.TryParse<Brainf_ckOperation>(Source!, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterResult>.From(validationResult);

            if (InitialState is TuringMachineState initialState)
            {
                Guard.MustBeNull(MemorySize, nameof(MemorySize));
                Guard.MustBeNull(OverflowMode, nameof(OverflowMode));

                initialState = (TuringMachineState)initialState.Clone();
            }
            else
            {
                int size = MemorySize ?? Specs.DefaultMemorySize;

                Guard.MustBeGreaterThanOrEqualTo(size, Specs.MinimumMemorySize, nameof(MemorySize));
                Guard.MustBeLessThanOrEqualTo(size, Specs.MaximumMemorySize, nameof(MemorySize));

                initialState = new TuringMachineState(size, OverflowMode ?? Specs.DefaultOverflowMode);
            }

            InterpreterResult result = Brainf_ckInterpreter.Release.Run(
                operations!.Span,
                Stdin ?? string.Empty,
                initialState,
                ExecutionToken);

            return Option<InterpreterResult>.From(validationResult, result);
        }
    }
}
