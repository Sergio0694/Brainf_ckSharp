using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Opcodes;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;

namespace Brainf_ckSharp.Configurations;

/// <summary>
/// A model for a RELEASE configuration being built
/// </summary>
public readonly ref partial struct ReleaseConfiguration
{
    /// <summary>
    /// Runs the current Brainf*ck/PBrain configuration
    /// </summary>
    /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Option<InterpreterResult> TryRun()
    {
        Guard.IsNotNull(this.Source);

        using MemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.TryParse<Brainf_ckOperation>(this.Source.Value.Span, out SyntaxValidationResult validationResult);

        if (!validationResult.IsSuccess)
        {
            return Option<InterpreterResult>.From(validationResult);
        }

        if (this.InitialState is TuringMachineState initialState)
        {
            Guard.IsNull(this.MemorySize);
            Guard.IsNull(this.DataType);

            initialState = (TuringMachineState)initialState.Clone();
        }
        else
        {
            int size = this.MemorySize ?? Specs.DefaultMemorySize;

            Guard.IsBetweenOrEqualTo(size, Specs.MinimumMemorySize, Specs.MaximumMemorySize, nameof(this.MemorySize));

            initialState = new TuringMachineState(size, this.DataType ?? Specs.DefaultDataType);
        }

        InterpreterResult result = Brainf_ckInterpreter.Release.Run(
            operations!.Span,
            this.Stdin.GetValueOrDefault().Span,
            initialState,
            this.IsOverflowEnabled,
            this.ExecutionToken);

        return Option<InterpreterResult>.From(validationResult, result);
    }
}
