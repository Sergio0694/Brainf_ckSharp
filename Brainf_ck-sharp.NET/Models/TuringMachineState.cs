using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Helpers;
using Brainf_ck_sharp.NET.Interfaces;
using Brainf_ck_sharp.NET.MemoryState;

#pragma warning disable IDE0032

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A <see langword="class"/> that represents the state of a Turing machine
    /// </summary>
    internal sealed unsafe class TuringMachineState : UnsafeMemoryBuffer<ushort>, IReadOnlyTuringMachineState
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

        /// <inheritdoc/>
        public int Position => _Position;

        /// <inheritdoc/>
        public int Count => Size;

        /// <inheritdoc/>
        public Brainf_ckMemoryCell Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Brainf_ckMemoryCell(Ptr[_Position], true);
        }

        /// <inheritdoc/>
        Brainf_ckMemoryCell IReadOnlyList<Brainf_ckMemoryCell>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Guard.MustBeGreaterThanOrEqualTo(index, 0, nameof(index));
                Guard.MustBeLessThan(index, Size, nameof(index));

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
        internal void MoveNext()
        {
            DebugGuard.MustBeTrue(CanMoveNext, nameof(CanMoveNext));

            _Position++;
        }

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
        internal void MoveBack()
        {
            DebugGuard.MustBeTrue(CanMoveBack, nameof(CanMoveBack));

            _Position--;
        }

        /// <summary>
        /// Checks whether or not it is possible to increment the current memory location
        /// </summary>
        internal bool CanIncrement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[_Position] < ushort.MaxValue;
        }

        /// <summary>
        /// Increments the current memory location
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Plus()
        {
            DebugGuard.MustBeTrue(CanIncrement, nameof(CanIncrement));

            Ptr[_Position]++;
        }

        /// <summary>
        /// Checks whether or not the current memory location is a positive number and can be decremented
        /// </summary>
        internal bool CanDecrement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[_Position] > 0;
        }

        /// <summary>
        /// Decrements the current memory location
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Minus()
        {
            DebugGuard.MustBeTrue(CanDecrement, nameof(CanDecrement));

            Ptr[_Position]--;
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
        public bool Equals(IReadOnlyTuringMachineState other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return other is TuringMachineState state &&
                   Size == state.Size &&
                   _Position == state._Position &&
                   new ReadOnlySpan<ushort>(Ptr, Size).SequenceEqual(new ReadOnlySpan<ushort>(state.Ptr, Size));
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
