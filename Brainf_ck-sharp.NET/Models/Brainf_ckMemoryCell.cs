using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Brainf_ck_sharp.NET.MemoryState
{
    /// <summary>
    /// A model that represents the information on a given memory cell in a <see cref="IReadonlyTouringMachineState"/> object
    /// </summary>
    public readonly struct Brainf_ckMemoryCell : IEquatable<Brainf_ckMemoryCell>, IComparable<Brainf_ckMemoryCell>
    {
        /// <summary>
        /// Gets whether or not the cell is currently selected
        /// </summary>
        public bool IsSelected { get; }

        /// <summary>
        /// Gets the value of the current cell
        /// </summary>
        public ushort Value { get; }

        /// <summary>
        /// Gets the corresponding character for the current cell
        /// </summary>
        public char Character
        {
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (char)Value;
        }

        /// <summary>
        /// Creates a new instance with the given value
        /// </summary>
        /// <param name="value">The value for the memory cell</param>
        /// <param name="isSelected">Gets whether the cell is selected</param>
        internal Brainf_ckMemoryCell(ushort value, bool isSelected)
        {
            Value = value;
            IsSelected = isSelected;
        }

        // Operators and equality operators
        public static bool operator ==(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => a.IsSelected == b.IsSelected && a.Value == b.Value;
        public static bool operator !=(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(Brainf_ckMemoryCell other) => this == other;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Brainf_ckMemoryCell cell && this == cell;

        /// <inheritdoc/>
        public int CompareTo(Brainf_ckMemoryCell other) => Value - other.Value;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            ref bool r0 = ref Unsafe.AsRef(IsSelected);
            ref byte r1 = ref Unsafe.As<bool, byte>(ref r0);

            return r1 << 16 | Value;
        }
    }
}
