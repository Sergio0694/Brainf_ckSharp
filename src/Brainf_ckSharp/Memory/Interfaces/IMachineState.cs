using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Opcodes;

namespace Brainf_ckSharp.Memory.Interfaces;

/// <summary>
/// An <see langword="interface"/> that is used to dispatch an execution on a machine state
/// </summary>
internal interface IMachineState : IReadOnlyMachineState
{
    /// <summary>
    /// Tries to run a given input Brainf*ck/PBrain executable
    /// </summary>
    /// <param name="executionOptions">The execution options to use when running the script</param>
    /// <param name="executionParameters">The execution parameters to use to run the script</param>
    /// <returns>The resulting <see cref="ExitCode"/> value for the current execution of the input script</returns>
    ExitCode Invoke(ExecutionOptions executionOptions, in ExecutionParameters<Brainf_ckOperation> executionParameters);

    /// <summary>
    /// Tries to run a given input Brainf*ck/PBrain executable
    /// </summary>
    /// <param name="executionOptions">The execution options to use when running the script</param>
    /// <param name="executionParameters">The execution parameters to use to run the script</param>
    /// <param name="debugParameters">The debug parameters to use to run the script</param>
    /// <returns>The resulting <see cref="ExitCode"/> value for the current execution of the input script</returns>
    ExitCode Invoke(
        ExecutionOptions executionOptions,
        in ExecutionParameters<Brainf_ckOperator> executionParameters,
        in DebugParameters debugParameters);
}