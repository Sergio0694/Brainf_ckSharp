using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;

namespace Brainf_ckSharp.Memory
{
    /// <summary>
    /// A <see langword="class"/> that represents the state of a Turing machine
    /// </summary>
    internal sealed unsafe partial class TuringMachineState : PinnedUnmanagedMemoryOwner<ushort>, IReadOnlyMachineState
    {
        /// <summary>
        /// The current position within the underlying buffer
        /// </summary>
        public int _Position;

        /// <summary>
        /// The overflow mode being used by the current instance
        /// </summary>
        public readonly OverflowMode Mode;

        /// <summary>
        /// Creates a new blank machine state with the given parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="mode">The overflow mode to use in the new instance</param>
        public TuringMachineState(int size, OverflowMode mode) : this(size, mode, true) { }

        /// <summary>
        /// Creates a new blank machine state with the given parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="mode">The overflow mode to use in the new instance</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        private TuringMachineState(int size, OverflowMode mode, bool clear) : base(size, clear)
        {
            Mode = mode;
        }

        /// <inheritdoc/>
        public int Position => _Position;

        /// <inheritdoc/>
        public int Count => Size;

        /// <inheritdoc/>
        public Brainf_ckMemoryCell Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Brainf_ckMemoryCell(_Position, Ptr[_Position], true);
        }

        /// <inheritdoc/>
        Brainf_ckMemoryCell IReadOnlyList<Brainf_ckMemoryCell>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Guard.MustBeGreaterThanOrEqualTo(index, 0, nameof(index));
                Guard.MustBeLessThan(index, Size, nameof(index));

                return new Brainf_ckMemoryCell(index, Ptr[index], _Position == index);
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) ||
                   obj is TuringMachineState other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(IReadOnlyMachineState other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                other is TuringMachineState state &&
                Size == state.Size &&
                Mode == state.Mode &&
                _Position == state._Position &&
                new ReadOnlySpan<ushort>(Ptr, Size).SequenceEqual(new ReadOnlySpan<ushort>(state.Ptr, Size));
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")] // Non immutable instance, hash code is allowed to change
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Size;
                hashCode = (hashCode * 397) ^ _Position;
                hashCode = (hashCode * 397) ^ (int)Mode;

                for (int i = 0; i < Size; i++)
                    hashCode = (hashCode * 397) ^ Ptr[i];

                return hashCode;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public IEnumerator<Brainf_ckMemoryCell> GetEnumerator()
        {
            for (int i = 0; i < Size; i++)
            {
                yield return new Brainf_ckMemoryCell(i, this[i], _Position == i);
            }
        }

        /// <inheritdoc/>
        public object Clone()
        {
            TuringMachineState clone = new TuringMachineState(Size, Mode, false) { _Position = _Position };

            new ReadOnlySpan<ushort>(Ptr, Size).CopyTo(new Span<ushort>(clone.Ptr, Size));

            return clone;
        }
    }
}
