using System;

namespace Brainf_ck_sharp.MemoryState
{
    /// <summary>
    /// Contains the information on a given memory cell in a <see cref="IReadonlyTouringMachineState"/> object
    /// </summary>
    public struct Brainf_ckMemoryCell
    {
        /// <summary>
        /// Gets the value of the current cell
        /// </summary>
        public uint Value { get; }

        /// <summary>
        /// Gets the corresponding character for the current cell
        /// </summary>
        public char Character => Convert.ToChar(Value);

        /// <summary>
        /// Creates a new instance with the given value
        /// </summary>
        /// <param name="value">The value for the memory cell</param>
        internal Brainf_ckMemoryCell(uint value) => Value = value;

        // Operators and equality operators
        public static bool operator ==(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => a.Value == b.Value;
        public static bool operator !=(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => a.Value != b.Value;

        /// <inheritdoc/>
        public bool Equals(Brainf_ckMemoryCell other) => Value == other.Value;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Brainf_ckMemoryCell cell) return Value == cell.Value;
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => (int)Value;
    }
}
