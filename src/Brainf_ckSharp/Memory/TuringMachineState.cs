using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace Brainf_ckSharp.Memory
{
    /// <summary>
    /// A <see langword="class"/> that represents the state of a Turing machine
    /// </summary>
    internal sealed partial class TuringMachineState : IReadOnlyMachineState
    {
        /// <summary>
        /// The size of the usable buffer within <see cref="Buffer"/>
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
        private ushort[]? Buffer;

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
            Buffer = ArrayPool<ushort>.Shared.Rent(size);
            Size = size;
            Mode = mode;

            if (clear)
            {
                Buffer.AsSpan(0, size).Clear();
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
                ushort[]? array = Buffer;

                if (array is null) ThrowObjectDisposedException();

                ushort value = array!.DangerousGetReferenceAt(_Position);

                return new Brainf_ckMemoryCell(_Position, value, true);
            }
        }

        /// <inheritdoc/>
        public Brainf_ckMemoryCell this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ushort[]? array = Buffer;

                if (array is null) ThrowObjectDisposedException();

                // Manually check the current size, as the buffer
                // is rented from the pool and its length might
                // actually be greater than the memory state.
                Guard.IsInRange(index, 0, Size, nameof(index));

                ushort value = array!.DangerousGetReferenceAt(index);

                return new Brainf_ckMemoryCell(index, value, _Position == index);
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return
                ReferenceEquals(this, obj) ||
                obj is TuringMachineState other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(IReadOnlyMachineState? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Buffer is null) ThrowObjectDisposedException();

            if (!(other is TuringMachineState state)) return false;

            if (state.Buffer is null) ThrowObjectDisposedException();

            return
                Size == state.Size &&
                Mode == state.Mode &&
                _Position == state._Position &&
                Buffer.AsSpan(0, Size).SequenceEqual(state.Buffer.AsSpan(0, Size));
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")] // Non immutable instance, hash code is allowed to change
        public override int GetHashCode()
        {
            if (Buffer is null) ThrowObjectDisposedException();

            unchecked
            {
                int hashCode = Size;
                hashCode = (hashCode * 397) ^ _Position;
                hashCode = (hashCode * 397) ^ (int)Mode;

                foreach (ushort value in Buffer.AsSpan())
                {
                    hashCode = (hashCode * 397) ^ value;
                }

                return hashCode;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Brainf_ckMemoryCell>)this).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<Brainf_ckMemoryCell> IEnumerable<Brainf_ckMemoryCell>.GetEnumerator()
        {
            ushort[]? array = Buffer;

            if (array is null) ThrowObjectDisposedException();

            for (int i = 0; i < Size; i++)
            {
                ushort value = array!.DangerousGetReferenceAt(i);

                yield return new Brainf_ckMemoryCell(i, value, _Position == i);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyMachineStateEnumerator GetEnumerator()
        {
            return new IReadOnlyMachineStateEnumerator(this);
        }

        /// <inheritdoc/>
        public object Clone()
        {
            if (Buffer is null) ThrowObjectDisposedException();

            TuringMachineState clone = new TuringMachineState(Size, Mode, false) { _Position = _Position };

            Buffer.AsSpan(0, Size).CopyTo(clone.Buffer.AsSpan(0, Size));

            return clone;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ushort[]? array = Buffer;

            if (array is null) return;

            Buffer = null;

            ArrayPool<ushort>.Shared.Return(array);
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> when <see cref="Buffer"/> is <see langword="null"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowObjectDisposedException()
        {
            throw new ObjectDisposedException("The current machine state has been disposed");
        }
    }
}
