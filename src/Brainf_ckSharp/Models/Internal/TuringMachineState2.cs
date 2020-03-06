using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Interfaces;

#pragma warning disable IDE0032

namespace Brainf_ckSharp.Models.Internal
{
    /// <summary>
    /// A <see langword="class"/> that represents the state of a Turing machine
    /// </summary>
    internal sealed unsafe partial class TuringMachineState2 : PinnedUnmanagedMemoryOwner<ushort>, IReadOnlyTuringMachineState
    {
        /// <summary>
        /// The current position within the underlying buffer
        /// </summary>
        private int _Position;

        /// <summary>
        /// The overflow mode being used by the current instance
        /// </summary>
        private readonly OverflowMode Mode;

        /// <summary>
        /// Creates a new blank machine state with the given parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="mode">The overflow mode to use in the new instance</param>
        public TuringMachineState2(int size, OverflowMode mode) : this(size, mode, true) { }

        /// <summary>
        /// Creates a new blank machine state with the given parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="mode">The overflow mode to use in the new instance</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        private TuringMachineState2(int size, OverflowMode mode, bool clear) : base(size, clear)
        {
            Mode = mode;
        }

        /// <inheritdoc/>
        public int Position => _Position;

        /// <inheritdoc/>
        public int Count => Size;

        /// <summary>
        /// Gets the value at the current memory position
        /// </summary>
        public ushort Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[_Position];
        }

        /// <inheritdoc/>
        Brainf_ckMemoryCell IReadOnlyTuringMachineState.Current
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
        public bool Equals(IReadOnlyTuringMachineState other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return other is TuringMachineState2 state &&
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
            TuringMachineState2 clone = new TuringMachineState2(Size, Mode, false) { _Position = _Position };

            new ReadOnlySpan<ushort>(Ptr, Size).CopyTo(new Span<ushort>(clone.Ptr, Size));

            return clone;
        }
    }

    internal interface IMachineStateExecutionContext
    {
        /// <summary>
        /// Tries to move the memory pointer forward
        /// </summary>
        /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>;
        bool TryMoveNext();

        /// <summary>
        /// Tries to move the memory pointer back
        /// </summary>
        /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>
        bool TryMoveBack();

        /// <summary>
        /// Tries to increment the current memory location
        /// </summary>
        /// <returns><see langword="true"/> if the memory location was incremented successfully, <see langword="false"/> otherwise</returns>
        bool TryIncrement();

        /// <summary>
        /// Tries to decrement the current memory location
        /// </summary>
        /// <returns><see langword="true"/> if the memory location was decremented successfully, <see langword="false"/> otherwise</returns>
        bool TryDecrement();

        /// <summary>
        /// Tries to set the current memory location to the value of a given character
        /// </summary>
        /// <param name="c">The input charachter to assign to the current memory location</param>
        /// <returns><see langword="true"/> if the input value was read correctly, <see langword="false"/> otherwise</returns>
        bool TryInput(char c);

        /// <summary>
        /// Resets the value in the current memory cell
        /// </summary>
        void ResetCell();
    }
}
