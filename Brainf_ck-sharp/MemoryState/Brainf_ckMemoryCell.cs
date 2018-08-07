using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.MemoryState
{
    /// <summary>
    /// Contains the information on a given memory cell in a <see cref="IReadonlyTouringMachineState"/> object
    /// </summary>
    public readonly struct Brainf_ckMemoryCell : IEquatable<Brainf_ckMemoryCell>
    {
        /// <summary>
        /// Gets whether or not the cell is currently selected
        /// </summary>
        public bool Selected { get; }

        /// <summary>
        /// Gets the value of the current cell
        /// </summary>
        public uint Value { get; }

        /// <summary>
        /// Gets the corresponding character for the current cell
        /// </summary>
        public char Character
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Convert.ToChar(Value);
        }

        /// <summary>
        /// Creates a new instance with the given value
        /// </summary>
        /// <param name="value">The value for the memory cell</param>
        /// <param name="selected">Gets whether the cell is selected</param>
        internal Brainf_ckMemoryCell(uint value, bool selected)
        {
            Value = value;
            Selected = selected;
        }

        // Operators and equality operators
        public static bool operator ==(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => a.Value == b.Value;
        public static bool operator !=(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => a.Value != b.Value;

        /// <inheritdoc/>
        public bool Equals(Brainf_ckMemoryCell other) => Selected == other.Selected && Value == other.Value;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Brainf_ckMemoryCell cell) return Equals(cell);
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Selected.GetHashCode() * 397) ^ (int)Value;
            }
        }
    }
}
