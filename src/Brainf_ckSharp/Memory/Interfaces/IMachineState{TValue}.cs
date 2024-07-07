using System.Numerics;

namespace Brainf_ckSharp.Memory.Interfaces;

/// <summary>
/// An <see langword="interface"/> that is used to dispatch an execution on a machine state
/// </summary>
/// <typeparam name="TValue">The type of values in each memory cell</typeparam>
internal interface IMachineState<TValue> : IMachineState
    where TValue : unmanaged, IBinaryInteger<TValue>
{
    /// <summary>
    /// Creates an execution context with the specified type arguments.
    /// </summary>
    /// <typeparam name="TSize">The type representing the size of the machine state</typeparam>
    /// <typeparam name="TNumberHandler">The type handling numeric operations for the machine state</typeparam>
    /// <returns>The execution context to use.</returns>
    ExecutionContext<TValue, TSize, TNumberHandler> CreateExecutionContext<TSize, TNumberHandler>()
        where TSize : unmanaged, IMachineStateSize
        where TNumberHandler : unmanaged, IMachineStateNumberHandler<TValue>;

    /// <summary>
    /// Finalizes the execution of a script from a given execution context.
    /// </summary>
    /// <typeparam name="TExecutionContext">The type of execution context used to run the script.</typeparam>
    /// <param name="executionContext">The execution context used to run the script.</param>
    void FinalizeExecution<TExecutionContext>(in TExecutionContext executionContext)
        where TExecutionContext : struct, IMachineStateExecutionContext, allows ref struct;
}