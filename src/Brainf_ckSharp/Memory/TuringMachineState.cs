using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.HighPerformance;

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
        _Buffer = ArrayPool<ushort>.Shared.Rent(size);
        Size = size;
        Mode = mode;

        if (clear)
        {
            _Buffer.AsSpan(0, size).Clear();
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="TuringMachineState"/> class.
    /// </summary>
    ~TuringMachineState() => Dispose();

    /// <inheritdoc/>
    public int Position => _Position;

    /// <inheritdoc/>
    public int Count => Size;

    /// <inheritdoc/>
    public Brainf_ckMemoryCell Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ushort[]? array = _Buffer;

            if (array is null) ThrowObjectDisposedException();

            ushort value = array!.DangerousGetReferenceAt(_Position);

            return new(_Position, value, true);
        }
    }

    /// <inheritdoc/>
    public Brainf_ckMemoryCell this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ushort[]? array = _Buffer;

            if (array is null) ThrowObjectDisposedException();

            // Manually check the current size, as the buffer
            // is rented from the pool and its length might
            // actually be greater than the memory state.
            Guard.IsInRange(index, 0, Size);

            ushort value = array!.DangerousGetReferenceAt(index);

            return new(index, value, _Position == index);
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

        if (_Buffer is null) ThrowObjectDisposedException();

        if (!(other is TuringMachineState state)) return false;

        if (state._Buffer is null) ThrowObjectDisposedException();

        return
            Size == state.Size &&
            Mode == state.Mode &&
            _Position == state._Position &&
            _Buffer.AsSpan(0, Size).SequenceEqual(state._Buffer.AsSpan(0, Size));
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (_Buffer is null) ThrowObjectDisposedException();

        HashCode hashCode = default;

        hashCode.Add(Size);
        hashCode.Add(Mode);
        hashCode.Add(_Position);
        hashCode.Add<ushort>(_Buffer.AsSpan(0, Size));

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
        for (int i = 0; i < Size; i++)
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
        if (_Buffer is null) ThrowObjectDisposedException();

        TuringMachineState clone = new(Size, Mode, false) { _Position = _Position };

        _Buffer.AsSpan(0, Size).CopyTo(clone._Buffer.AsSpan(0, Size));

        return clone;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ushort[]? array = _Buffer;

        if (array is null) return;

        _Buffer = null;

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
