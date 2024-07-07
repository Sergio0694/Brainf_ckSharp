using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes;
using CommunityToolkit.HighPerformance;

namespace Brainf_ckSharp.Memory;

/// <summary>
/// A <see langword="class"/> that represents the state of a Turing machine
/// </summary>
/// <typeparam name="TValue">The type of values in each memory cell</typeparam>
internal sealed partial class TuringMachineState<TValue> : IMachineState<TValue>
    where TValue : unmanaged, IBinaryInteger<TValue>, IMinMaxValue<TValue>
{
    /// <summary>
    /// The size of the machine state
    /// </summary>
    private readonly int size;

    /// <summary>
    /// The underlying <typeparamref name="TValue"/> buffer
    /// </summary>
    /// <remarks>
    /// Similarly to <see cref="Buffers.StdoutBuffer"/>, the buffer is rented directly
    /// from this type to reduce the overhead when accessing individual items.
    /// </remarks>
    private TValue[]? buffer;

    /// <summary>
    /// The current position within the underlying buffer.
    /// </summary>
    private int position;

    /// <summary>
    /// Creates a new blank machine state with the given parameters
    /// </summary>
    /// <param name="size">The size of the new memory buffer to use</param>
    public TuringMachineState(int size)
        : this(size, true)
    {
    }

    /// <summary>
    /// Creates a new blank machine state with the given parameters
    /// </summary>
    /// <param name="size">The size of the new memory buffer to use</param>
    /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
    private TuringMachineState(int size, bool clear)
    {
        this.size = size;
        this.buffer = ArrayPool<TValue>.Shared.Rent(size);
        this.position = 0;

        if (clear)
        {
            this.buffer.AsSpan(0, size).Clear();
        }
    }

    /// <inheritdoc/>
    public int Position => this.position;

    /// <inheritdoc/>
    public int Count => this.size;

    /// <inheritdoc/>
    public Brainf_ckMemoryCell Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            TValue[]? array = this.buffer;

            ObjectDisposedException.ThrowIf(array is null, this);

            TValue value = array.DangerousGetReferenceAt(this.position);

            return new(this.position, ushort.CreateTruncating(value), true);
        }
    }

    /// <inheritdoc/>
    public Brainf_ckMemoryCell this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            TValue[]? array = this.buffer;

            ObjectDisposedException.ThrowIf(array is null, this);

            // Manually check the current size, as the buffer
            // is rented from the pool and its length might
            // actually be greater than the memory state.
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.size);

            TValue value = array.DangerousGetReferenceAt(index);

            return new(index, ushort.CreateTruncating(value), this.position == index);
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as IReadOnlyMachineState);
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyMachineState? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        ObjectDisposedException.ThrowIf(this.buffer is null, this);

        if (other is not TuringMachineState<TValue> state)
        {
            return false;
        }

        ObjectDisposedException.ThrowIf(state.buffer is null, state);

        return
            this.size == state.size &&
            this.position == state.position &&
            this.buffer.AsSpan(0, this.size).SequenceEqual(state.buffer.AsSpan(0, this.size));
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        ObjectDisposedException.ThrowIf(this.buffer is null, this);

        HashCode hashCode = default;

        hashCode.Add(typeof(TValue));
        hashCode.Add(this.size);
        hashCode.Add(this.position);
        hashCode.Add<TValue>(this.buffer.AsSpan(0, this.size));

        return hashCode.ToHashCode();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Brainf_ckMemoryCell>)this).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator<Brainf_ckMemoryCell> IEnumerable<Brainf_ckMemoryCell>.GetEnumerator()
    {
        for (int i = 0; i < this.size; i++)
        {
            yield return this[i];
        }
    }

    /// <inheritdoc/>
    public IReadOnlyMachineStateEnumerator GetEnumerator()
    {
        return new(this);
    }

    /// <inheritdoc/>
    public object Clone()
    {
        ObjectDisposedException.ThrowIf(this.buffer is null, this);

        TuringMachineState<TValue> clone = new(this.size, false) { position = this.position };

        this.buffer.AsSpan(0, this.size).CopyTo(clone.buffer.AsSpan(0, this.size));

        return clone;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        TValue[]? array = this.buffer;

        if (array is null)
        {
            return;
        }

        this.buffer = null;

        ArrayPool<TValue>.Shared.Return(array);
    }

    /// <inheritdoc/>
    ExitCode IMachineState.Invoke(ExecutionOptions executionOptions, in ExecutionParameters<Brainf_ckOperation> executionParameters)
    {
        return Brainf_ckInterpreter.Release.Run(this, executionOptions, in executionParameters);
    }

    /// <inheritdoc/>
    ExitCode IMachineState.Invoke(
        ExecutionOptions executionOptions,
        in ExecutionParameters<Brainf_ckOperator> executionParameters,
        in DebugParameters debugParameters)
    {
        return Brainf_ckInterpreter.Debug.Run(this, executionOptions, in executionParameters, in debugParameters);
    }

    /// <inheritdoc/>
    MachineStateExecutionContext<TValue, TSize, TNumberHandler> IMachineState<TValue>.CreateExecutionContext<TSize, TNumberHandler>()
    {
        return new(ref this.buffer!.DangerousGetReference(), this.size);
    }

    /// <inheritdoc/>
    void IMachineState<TValue>.FinalizeExecution<TExecutionContext>(in TExecutionContext executionContext)
    {
        this.position = executionContext.Position;
    }
}
