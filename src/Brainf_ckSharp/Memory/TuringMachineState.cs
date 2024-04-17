using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;

namespace Brainf_ckSharp.Memory;

/// <summary>
/// A <see langword="class"/> that represents the state of a Turing machine
/// </summary>
internal sealed partial class TuringMachineState : IReadOnlyMachineState
{
    /// <summary>
    /// The size of the usable buffer within <see cref="_Buffer"/>
    /// </summary>
    public readonly int Size;

    /// <summary>
    /// The overflow mode being used by the current instance
    /// </summary>
    public readonly OverflowMode Mode;

    /// <summary>
    /// The underlying <see cref="ushort"/> buffer
    /// </summary>
    /// <remarks>
    /// Similarly to <see cref="Buffers.StdoutBuffer"/>, the buffer is rented directly
    /// from this type to reduce the overhead when accessing individual items.
    /// </remarks>
    private ushort[]? _Buffer;

    /// <summary>
    /// The current position within the underlying buffer
    /// </summary>
    public int _Position;

    /// <summary>
    /// Creates a new blank machine state with the given parameters
    /// </summary>
    /// <param name="size">The size of the new memory buffer to use</param>
    /// <param name="mode">The overflow mode to use in the new instance</param>
    public TuringMachineState(int size, OverflowMode mode)
        : this(size, mode, true)
    { }

    /// <summary>
    /// Creates a new blank machine state with the given parameters
    /// </summary>
    /// <param name="size">The size of the new memory buffer to use</param>
    /// <param name="mode">The overflow mode to use in the new instance</param>
    /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
    private TuringMachineState(int size, OverflowMode mode, bool clear)
    {
        this._Buffer = ArrayPool<ushort>.Shared.Rent(size);
        this.Size = size;
        this.Mode = mode;

        if (clear)
        {
            this._Buffer.AsSpan(0, size).Clear();
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="TuringMachineState"/> class.
    /// </summary>
    ~TuringMachineState() => Dispose();

    /// <inheritdoc/>
    public int Position => this._Position;

    /// <inheritdoc/>
    public int Count => this.Size;

    /// <inheritdoc/>
    public Brainf_ckMemoryCell Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ushort[]? array = this._Buffer;

            if (array is null) ThrowObjectDisposedException();

            ushort value = array!.DangerousGetReferenceAt(this._Position);

            return new(this._Position, value, true);
        }
    }

    /// <inheritdoc/>
    public Brainf_ckMemoryCell this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ushort[]? array = this._Buffer;

            if (array is null) ThrowObjectDisposedException();

            // Manually check the current size, as the buffer
            // is rented from the pool and its length might
            // actually be greater than the memory state.
            Guard.IsInRange(index, 0, this.Size);

            ushort value = array!.DangerousGetReferenceAt(index);

            return new(index, value, this._Position == index);
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
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        if (this._Buffer is null) ThrowObjectDisposedException();

        if (other is not TuringMachineState state) return false;

        if (state._Buffer is null) ThrowObjectDisposedException();

        return
            this.Size == state.Size &&
            this.Mode == state.Mode &&
            this._Position == state._Position &&
            this._Buffer.AsSpan(0, this.Size).SequenceEqual(state._Buffer.AsSpan(0, this.Size));
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (this._Buffer is null) ThrowObjectDisposedException();

        HashCode hashCode = default;

        hashCode.Add(this.Size);
        hashCode.Add(this.Mode);
        hashCode.Add(this._Position);
        hashCode.Add<ushort>(this._Buffer.AsSpan(0, this.Size));

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
        for (int i = 0; i < this.Size; i++)
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
        if (this._Buffer is null) ThrowObjectDisposedException();

        TuringMachineState clone = new(this.Size, this.Mode, false) { _Position = this._Position };

        this._Buffer.AsSpan(0, this.Size).CopyTo(clone._Buffer.AsSpan(0, this.Size));

        return clone;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ushort[]? array = this._Buffer;

        if (array is null) return;

        this._Buffer = null;

        ArrayPool<ushort>.Shared.Return(array);
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> when <see cref="_Buffer"/> is <see langword="null"/>.
    /// </summary>
    private static void ThrowObjectDisposedException()
    {
        throw new ObjectDisposedException(nameof(IReadOnlyMachineState), "The current machine state has been disposed");
    }
}
