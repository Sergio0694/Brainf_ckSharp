using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Brainf_ckSharp.Legacy.MemoryState
{
    /// <summary>
    /// A class that represents the state of a Touring machine (data + position)
    /// </summary>
    internal sealed class TouringMachineState : IReadonlyTouringMachineState
    {
        /// <summary>
        /// The unsigned int array that represents the memory of the Touring machine
        /// </summary>
        [NotNull]
        private readonly ushort[] Memory;

        /// <inheritdoc/>
        public int Position { get; private set; }

        /// <inheritdoc/>
        public int Count { get; }

        /// <summary>
        /// Creates a new blank machine state with the given parameters
        /// </summary>
        public TouringMachineState(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "The size must be a positive number");
            if (size > 4096) throw new ArgumentOutOfRangeException(nameof(size), "The size can't be greater than 4096");
            Count = size;
            Memory = new ushort[size];
        }

        /// <summary>
        /// Creates a new instance for the <see cref="Clone()"/> method
        /// </summary>
        /// <param name="memory">The source memory array</param>
        private TouringMachineState([NotNull] ushort[] memory)
        {
            ushort[] copy = new ushort[memory.Length];
            Buffer.BlockCopy(memory, 0, copy, 0, sizeof(ushort) * memory.Length);
            Memory = copy;
            Count = memory.Length;
        }

        /// <summary>
        /// Sets the current memory location to the value of a given character
        /// </summary>
        /// <param name="c">The input charachter to assign to the current memory location</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Input(char c) => Memory[Position] = c;

        /// <summary>
        /// Gets the value of the current memory position (*ptr)
        /// </summary>
        public Brainf_ckMemoryCell Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Brainf_ckMemoryCell(Memory[Position], true);
        }

        /// <summary>
        /// Checks whether or not it is possible to move the pointer forward
        /// </summary>
        public bool CanMoveNext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position < Count - 1;
        }

        /// <summary>
        /// Moves the memory pointer forward
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveNext() => Position++;

        /// <summary>
        /// Checks whether or not it is possible to move the pointer back
        /// </summary>
        public bool CanMoveBack
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position > 0;
        }

        /// <summary>
        /// Moves the memory pointer back
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveBack() => Position--;

        /// <summary>
        /// Increments the current memory location (*ptr++)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Plus() => Memory[Position]++;

        /// <summary>
        /// Checks whether or not it is possible to increment the current memory location
        /// </summary>
        public bool CanIncrement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Memory[Position] < ushort.MaxValue;
        }

        /// <summary>
        /// Decrements the current memory location (*ptr--)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Minus() => Memory[Position]--;

        /// <summary>
        /// Checks whether or not the current memory location is a positive number and can be decremented
        /// </summary>
        public bool CanDecrement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Memory[Position] > 0;
        }

        /// <summary>
        /// Resets the value in the current memory cell
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetCell() => Memory[Position] = 0;

        /// <summary>
        /// Creates a copy of the current machine state
        /// </summary>
        [Pure, NotNull]
        internal TouringMachineState Clone() => new TouringMachineState(Memory) { Position = Position };

        #region Overflow

        /// <inheritdoc/>
        public IReadonlyTouringMachineState ApplyByteOverflow()
        {
            // Quickly copy the current state without values greater than 255 (replace with Parallel.For when supported)
            ushort[] copy = new ushort[Count];
            unsafe
            {
                fixed (ushort* cp = copy, mp = Memory)
                    for (int i = 0; i < Count; i++)
                        cp[i] = mp[i] > byte.MaxValue
                            ? (ushort)(mp[i] % byte.MaxValue)
                            : mp[i];
            }
            return new TouringMachineState(copy) { Position = Position };
        }

        /// <summary>
        /// Gets whether or not the current cell is at 255
        /// </summary>
        internal bool IsAtByteMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Memory[Position] == byte.MaxValue;
        }

        #endregion

        #region IReadOnlyList<ushort>

        /// <inheritdoc/>
        public IEnumerator<Brainf_ckMemoryCell> GetEnumerator() => Memory.Select((m, i) => new Brainf_ckMemoryCell(m, i == Position)).GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => Memory.GetEnumerator();

        /// <inheritdoc/>
        public Brainf_ckMemoryCell this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Brainf_ckMemoryCell(Memory[index], index == Position);
        }

        #endregion
    }

    /// <summary>
    /// A simple static class that exposes a method to initialize a new machine state
    /// </summary>
    public static class TouringMachineStateProvider
    {
        /// <summary>
        /// Initializes a new memory state with all the cells set to 0
        /// </summary>
        /// <param name="size">The size of the new memory state</param>
        public static IReadonlyTouringMachineState Initialize(int size) => new TouringMachineState(size);
    }
}