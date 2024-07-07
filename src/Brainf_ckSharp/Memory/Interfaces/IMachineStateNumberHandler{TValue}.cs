#pragma warning disable CS1573

namespace Brainf_ckSharp.Memory.Interfaces;

/// <summary>
/// An <see langword="interface"/> that is used by mode-specific execution contexts
/// </summary>
/// <typeparam name="TValue">The type of values in each memory cell</typeparam>
internal interface IMachineStateNumberHandler<TValue>
{
    /// <inheritdoc cref="IMachineStateExecutionContext{TValue}.TryIncrement()"/>
    /// <param name="value">The target value to modify.</param>
    static abstract bool TryIncrement(ref TValue value);

    /// <inheritdoc cref="IMachineStateExecutionContext{TValue}.TryIncrement(int, ref int)"/>
    /// <param name="value">The target value to modify.</param>
    static abstract bool TryIncrement(ref TValue value, int count, ref int totalOperations);

    /// <inheritdoc cref="IMachineStateExecutionContext{TValue}.TryDecrement()"/>
    /// <param name="value">The target value to modify.</param>
    static abstract bool TryDecrement(ref TValue value);

    /// <inheritdoc cref="IMachineStateExecutionContext{TValue}.TryDecrement(int, ref int)"/>
    /// <param name="value">The target value to modify.</param>
    static abstract bool TryDecrement(ref TValue value, int count, ref int totalOperations);

    /// <inheritdoc cref="IMachineStateExecutionContext{TValue}.TryInput"/>
    /// <param name="value">The target value to modify.</param>
    static abstract bool TryInput(ref TValue value, char c);
}
