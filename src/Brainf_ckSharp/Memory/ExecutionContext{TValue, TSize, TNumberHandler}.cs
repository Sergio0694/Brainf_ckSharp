using System.Numerics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Memory.Interfaces;

namespace Brainf_ckSharp.Memory;

/// <summary>
/// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> with a given configuration
/// </summary>
/// <typeparam name="TValue">The type of values in each memory cell</typeparam>
/// <typeparam name="TSize">The type representing the size of the machine state</typeparam>
/// <typeparam name="TNumberHandler">The type handling numeric operations for the machine state</typeparam>
internal ref struct ExecutionContext<TValue, TSize, TNumberHandler> : IMachineStateExecutionContext
    where TValue : unmanaged, IBinaryInteger<TValue>
    where TSize : unmanaged, IMachineStateSize
    where TNumberHandler : unmanaged, IMachineStateNumberHandler<TValue>
{
    /// <summary>
    /// The reference to the machine state data
    /// </summary>
    private readonly ref TValue reference;

    /// <summary>
    /// The current position in the machine state data
    /// </summary>
    private int position;

    /// <summary>
    /// Creates a new <see cref="ExecutionContext{TValue, TSize, TNumberHandler}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="reference">The reference to the machine state data</param>
    /// <param name="position">The current position in the machine state data</param>
    public ExecutionContext(ref TValue reference, int position)
    {
        this.reference = ref reference;
        this.position = position;
    }

    /// <inheritdoc/>
    public readonly int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.position;
    }

    /// <inheritdoc/>
    public readonly ushort CurrentValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ushort.CreateTruncating(Unsafe.Add(ref this.reference, (uint)this.position));
    }

    /// <inheritdoc/>
    public readonly char CurrentCharacter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (char)ushort.CreateTruncating(Unsafe.Add(ref this.reference, (uint)this.position));
    }

    /// <inheritdoc/>
    public readonly bool IsCurrentValuePositive
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Unsafe.Add(ref this.reference, (uint)this.position) > TValue.Zero;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryMoveNext()
    {
        if (this.position != TSize.Value)
        {
            this.position++;

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryMoveNext(int count, ref int totalOperations)
    {
        if (this.position + count <= TSize.Value)
        {
            totalOperations += count;

            this.position += count;

            return true;
        }

        totalOperations += TSize.Value - this.position;

        this.position = TSize.Value;

        return false;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryMoveBack()
    {
        if (this.position != 0)
        {
            this.position--;

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryMoveBack(int count, ref int totalOperations)
    {
        if (this.position - count >= 0)
        {
            totalOperations += count;

            this.position -= count;

            return true;
        }

        totalOperations += this.position;

        this.position = 0;

        return false;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryIncrement()
    {
        ref TValue value = ref Unsafe.Add(ref this.reference, (uint)this.position);

        return TNumberHandler.TryIncrement(ref value);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryIncrement(int count, ref int totalOperations)
    {
        ref TValue value = ref Unsafe.Add(ref this.reference, (uint)this.position);

        return TNumberHandler.TryIncrement(ref value, count, ref totalOperations);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryDecrement()
    {
        ref TValue value = ref Unsafe.Add(ref this.reference, (uint)this.position);

        return TNumberHandler.TryDecrement(ref value);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryDecrement(int count, ref int totalOperations)
    {
        ref TValue value = ref Unsafe.Add(ref this.reference, (uint)this.position);

        return TNumberHandler.TryDecrement(ref value, count, ref totalOperations);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryInput(char c)
    {
        ref TValue value = ref Unsafe.Add(ref this.reference, (uint)this.position);

        return TNumberHandler.TryInput(ref value, c);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void ResetCell()
    {
        ref TValue value = ref Unsafe.Add(ref this.reference, (uint)this.position);

        value = TValue.Zero;
    }
}
