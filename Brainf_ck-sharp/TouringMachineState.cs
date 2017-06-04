using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    /// <summary>
    /// A class that represents the state of a Touring machine (data + position)
    /// </summary>
    public sealed class TouringMachineState : IReadOnlyList<ushort>
    {
        /// <summary>
        /// The unsigned int array that represents the memory of the Touring machine
        /// </summary>
        [NotNull]
        private readonly ushort[] Memory;

        /// <summary>
        /// The current position on the memory array
        /// </summary>
        private int _Position;

        /// <summary>
        /// Gets the size of the Touring machine memory
        /// </summary>
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
        /// Creates a new instance for the <see cref="Clone"/> method
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
        public void Input(char c) => Memory[_Position] = c;

        /// <summary>
        /// Gets the value of the current memory position (*ptr)
        /// </summary>
        public uint Current => Memory[_Position];

        /// <summary>
        /// Checks whether or not it is possible to move the pointer forward
        /// </summary>
        public bool CanMoveNext => _Position < Count - 1;

        /// <summary>
        /// Moves the memory pointer forward
        /// </summary>
        public void MoveNext() => _Position++;

        /// <summary>
        /// Checks whether or not it is possible to move the pointer back
        /// </summary>
        public bool CanMoveBack => _Position > 0;

        /// <summary>
        /// Moves the memory pointer back
        /// </summary>
        public void MoveBack() => _Position--;

        /// <summary>
        /// Increments the current memory location (*ptr++)
        /// </summary>
        public void Plus() => Memory[_Position]++;

        /// <summary>
        /// Checks whether or not it is possible to increment the current memory location
        /// </summary>
        public bool CanIncrement => Memory[_Position] < ushort.MaxValue;

        /// <summary>
        /// Decrements the current memory location (*ptr--)
        /// </summary>
        public void Minus() => Memory[_Position]--;

        /// <summary>
        /// Checks whether or not the current memory location is a positive number and can be decremented
        /// </summary>
        public bool CanDecrement => Memory[_Position] > 0;

        /// <summary>
        /// Creates a copy of the current machine state
        /// </summary>
        [Pure, NotNull]
        public TouringMachineState Clone() => new TouringMachineState(Memory) { _Position = _Position };

        #region IReadOnlyList<ushort>

        /// <inheritdoc cref="IEnumerable{T}"/>
        public IEnumerator<ushort> GetEnumerator() => (Memory as IEnumerable<ushort>).GetEnumerator();

        /// <inheritdoc cref="IEnumerable"/>
        IEnumerator IEnumerable.GetEnumerator() => Memory.GetEnumerator();

        /// <inheritdoc cref="IReadOnlyList{T}"/>
        public ushort this[int index] => Memory[index];

        #endregion
    }
}