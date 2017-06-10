using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.MemoryState
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

        /// <inheritdoc cref="IReadonlyTouringMachineState"/>
        public int Position { get; private set; }

        /// <inheritdoc cref="IReadOnlyList{T}"/>
        public int Count { get; }

        /// <summary>
        /// Creates a new blank machine state with the given parameters
        /// </summary>
        public TouringMachineState(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException("The size must be a positive number");
            if (size > 4096) throw new ArgumentOutOfRangeException("The size can't be greater than 4096");
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
        public void Input(char c) => Memory[Position] = c;

        /// <summary>
        /// Gets the value of the current memory position (*ptr)
        /// </summary>
        public Brainf_ckMemoryCell Current => new Brainf_ckMemoryCell(Memory[Position]);

        /// <summary>
        /// Checks whether or not it is possible to move the pointer forward
        /// </summary>
        public bool CanMoveNext => Position < Count - 1;

        /// <summary>
        /// Moves the memory pointer forward
        /// </summary>
        public void MoveNext() => Position++;

        /// <summary>
        /// Checks whether or not it is possible to move the pointer back
        /// </summary>
        public bool CanMoveBack => Position > 0;

        /// <summary>
        /// Moves the memory pointer back
        /// </summary>
        public void MoveBack() => Position--;

        /// <summary>
        /// Increments the current memory location (*ptr++)
        /// </summary>
        public void Plus() => Memory[Position]++;

        /// <summary>
        /// Checks whether or not it is possible to increment the current memory location
        /// </summary>
        public bool CanIncrement => Memory[Position] < ushort.MaxValue;

        /// <summary>
        /// Decrements the current memory location (*ptr--)
        /// </summary>
        public void Minus() => Memory[Position]--;

        /// <summary>
        /// Checks whether or not the current memory location is a positive number and can be decremented
        /// </summary>
        public bool CanDecrement => Memory[Position] > 0;

        /// <summary>
        /// Creates a copy of the current machine state
        /// </summary>
        [Pure, NotNull]
        internal TouringMachineState Clone() => new TouringMachineState(Memory) { Position = Position };

        #region IReadOnlyList<ushort>

        /// <inheritdoc cref="IEnumerable{T}"/>
        public IEnumerator<Brainf_ckMemoryCell> GetEnumerator() => Memory.Select(m => new Brainf_ckMemoryCell(m)).GetEnumerator();

        /// <inheritdoc cref="IEnumerable"/>
        IEnumerator IEnumerable.GetEnumerator() => Memory.GetEnumerator();

        /// <inheritdoc cref="IReadOnlyList{T}"/>
        public Brainf_ckMemoryCell this[int index] => new Brainf_ckMemoryCell(Memory[index]);

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