using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Memory.Tools;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Opcodes;
using Microsoft.Toolkit.Diagnostics;
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
            Guard.IsNotNull(Source, nameof(Source));

            using MemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.TryParse<Brainf_ckOperation>(Source.Value.Span, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterResult>.From(validationResult);

            if (InitialState is IMachineState initialState)
            {
                Guard.IsNull(MemorySize, nameof(MemorySize));
                Guard.IsNull(OverflowMode, nameof(OverflowMode));

                initialState = (IMachineState)initialState.Clone();
            }
            else
            {
                int size = MemorySize ?? Specs.DefaultMemorySize;

                Guard.IsBetweenOrEqualTo(size, Specs.MinimumMemorySize, Specs.MaximumMemorySize, nameof(MemorySize));

                initialState = (IMachineState)MachineStateProvider.Create(size, OverflowMode ?? Specs.DefaultOverflowMode);
            }

            InterpreterResult result = initialState.Run(
                operations!.Span,
                Stdin ?? string.Empty,
                ExecutionToken);

            return Option<InterpreterResult>.From(validationResult, result);
        }
    }
}
