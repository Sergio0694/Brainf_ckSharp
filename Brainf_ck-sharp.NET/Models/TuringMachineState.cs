using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.MemoryState;

#pragma warning disable IDE0032

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A class that represents the state of a Touring machine (data + position)
    /// </summary>
    internal sealed unsafe class TuringMachineState : UnsafeMemoryBuffer<ushort>, IEquatable<TuringMachineState>, IReadOnlyList<Brainf_ckMemoryCell>
    {
        /// <summary>
        /// The current position within the underlying buffer
        /// </summary>
        private int _Position;

        /// <summary>
        /// Creates a new blank machine state with the given parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        public TuringMachineState(int size) : base(size) { }

        /// <summary>
        /// Gets the current position on the memory buffer
        /// </summary>
        public int Position => _Position;

        /// <inheritdoc/>
        public int Count => Size;

        /// <summary>
        /// Gets the cell at the current memory position
        /// </summary>
        public Brainf_ckMemoryCell Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Brainf_ckMemoryCell(Ptr[_Position], true);
        }

        /// <inheritdoc/>
        public Brainf_ckMemoryCell this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), "The input index was invalid");

                return new Brainf_ckMemoryCell(Ptr[index], _Position == index);
            }
        }

        /// <summary>
        /// Sets the current memory location to the value of a given character
        /// </summary>
        /// <param name="c">The input charachter to assign to the current memory location</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Input(char c) => Ptr[_Position] = c;

        /// <summary>
        /// Checks whether or not it is possible to move the pointer forward
        /// </summary>
        internal bool CanMoveNext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Position < Size - 1;
        }

        /// <summary>
        /// Moves the memory pointer forward
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MoveNext() => _Position++;

        /// <summary>
        /// Checks whether or not it is possible to move the pointer back
        /// </summary>
        internal bool CanMoveBack
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Position > 0;
        }

        /// <summary>
        /// Moves the memory pointer back
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MoveBack() => _Position--;

        /// <summary>
        /// Increments the current memory location
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Plus() => Ptr[_Position]++;

        /// <summary>
        /// Checks whether or not it is possible to increment the current memory location
        /// </summary>
        internal bool CanIncrement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[_Position] < ushort.MaxValue;
        }

        /// <summary>
        /// Decrements the current memory location
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Minus() => Ptr[_Position]--;

        /// <summary>
        /// Checks whether or not the current memory location is a positive number and can be decremented
        /// </summary>
        internal bool CanDecrement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[_Position] > 0;
        }

        /// <summary>
        /// Resets the value in the current memory cell
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ResetCell() => Ptr[_Position] = 0;

        /// <summary>
        /// Gets whether or not the current cell is at 255
        /// </summary>
        internal bool IsAtByteMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[_Position] == 255;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) ||
                   obj is TuringMachineState other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(TuringMachineState other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Size == other.Size &&
                   _Position == other._Position &&
                   new ReadOnlySpan<ushort>(Ptr, Size).SequenceEqual(new ReadOnlySpan<ushort>(other.Ptr, Size));
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")] // Non immutable instance, hash code is allowed to change
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Size;
                hashCode = (hashCode * 397) ^ _Position;

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
            // Iterators don't allow unsafe code, so bounds checks can't be removed here
            for (int i = 0; i < Size; i++)
                yield return new Brainf_ckMemoryCell(Memory[i], _Position == i);
        }
    }
}
