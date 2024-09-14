using System.Runtime.CompilerServices;
using Brainf_ckSharp.Configurations;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Models;
using CommunityToolkit.Diagnostics;
using Brainf_ckSharp.Opcodes;
using CommunityToolkit.HighPerformance.Buffers;

namespace Brainf_ckSharp;

/// <summary>
/// A <see langword="class"/> responsible for interpreting and debugging Brainf*ck/PBrain scripts
/// </summary>
public static partial class Brainf_ckInterpreter
{
    /// <summary>
    /// Runs the current Brainf*ck/PBrain configuration
    /// </summary>
    /// <param name="configuration">The configuration to run</param>
    /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Option<InterpreterSession> TryRun(in DebugConfiguration configuration)
    {
        if (configuration.InitialState is TuringMachineState initialState)
        {
            Guard.IsNull(configuration.MemorySize);
            Guard.IsNull(configuration.DataType);

            initialState = (TuringMachineState)initialState.Clone();
        }
        else
        {
            int size = configuration.MemorySize ?? Specs.DefaultMemorySize;

            Guard.IsBetweenOrEqualTo(size, Specs.MinimumMemorySize, Specs.MaximumMemorySize, nameof(configuration.MemorySize));

            initialState = new TuringMachineState(size, configuration.DataType ?? Specs.DefaultDataType);
        }

        return Debug.TryCreateSession(
            configuration.Source.Span,
            configuration.Breakpoints.Span,
            configuration.Stdin,
            initialState,
            configuration.ExecutionOptions,
            configuration.ExecutionToken,
            configuration.DebugToken);
    }

    /// <summary>
    /// Runs the current Brainf*ck/PBrain configuration
    /// </summary>
    /// <param name="configuration">The configuration to run</param>
    /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Option<InterpreterResult> TryRun(in ReleaseConfiguration configuration)
    {
        using MemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.TryParse<Brainf_ckOperation>(
            configuration.Source.Span,
            out SyntaxValidationResult validationResult);

        if (!validationResult.IsSuccess)
        {
            return Option<InterpreterResult>.From(validationResult);
        }

        if (configuration.InitialState is TuringMachineState initialState)
        {
            Guard.IsNull(configuration.MemorySize);
            Guard.IsNull(configuration.DataType);

            initialState = (TuringMachineState)initialState.Clone();
        }
        else
        {
            int size = configuration.MemorySize ?? Specs.DefaultMemorySize;

            Guard.IsBetweenOrEqualTo(size, Specs.MinimumMemorySize, Specs.MaximumMemorySize, nameof(configuration.MemorySize));

            initialState = new TuringMachineState(size, configuration.DataType ?? Specs.DefaultDataType);
        }

        InterpreterResult result = Release.Run(
            operations!.Span,
            configuration.Stdin.Span,
            initialState,
            configuration.ExecutionOptions,
            configuration.ExecutionToken);

        return Option<InterpreterResult>.From(validationResult, result);
    }
}
