using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A model that represents the information on a given memory cell in a <see cref="Interfaces.IReadOnlyTuringMachineState"/> object
    /// </summary>
    public readonly struct Brainf_ckMemoryCell : IEquatable<Brainf_ckMemoryCell>
    {
        /// <summary>
        /// Gets the numerical index for the current memory cell
        /// </summary>
        public int Index { get; }

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

        private readonly bool _IsSelected;

        /// <summary>
        /// Gets whether or not the cell is currently selected
        /// </summary>
        public bool IsSelected => _IsSelected;

        /// <summary>
        /// Creates a new instance with the given value
        /// </summary>
        /// <param name="index">The index for the memory cell</param>
        /// <param name="value">The value for the memory cell</param>
        /// <param name="isSelected">Gets whether the cell is selected</param>
        internal Brainf_ckMemoryCell(int index, ushort value, bool isSelected)
        {
            Index = index;
            Value = value;
            _IsSelected = isSelected;
        }

        /// <summary>
        /// Checks whether or not two <see cref="Brainf_ckMemoryCell"/> instances are equal
        /// </summary>
        /// <param name="a">The first <see cref="Brainf_ckMemoryCell"/> instance to compare</param>
        /// <param name="b">The second <see cref="Brainf_ckMemoryCell"/> instance to compare</param>
        /// <returns><see langword="true"/> if the two input <see cref="Brainf_ckMemoryCell"/> are equal, <see langword="false"/> otherwise</returns>
        public static bool operator ==(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => a.Equals(b);

        /// <summary>
        /// Checks whether or not two <see cref="Brainf_ckMemoryCell"/> instances are not equal
        /// </summary>
        /// <param name="a">The first <see cref="Brainf_ckMemoryCell"/> instance to compare</param>
        /// <param name="b">The second <see cref="Brainf_ckMemoryCell"/> instance to compare</param>
        /// <returns><see langword="true"/> if the two input <see cref="Brainf_ckMemoryCell"/> are not equal, <see langword="false"/> otherwise</returns>
        public static bool operator !=(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => !a.Equals(b);

        /// <inheritdoc/>
        public bool Equals(Brainf_ckMemoryCell other)
        {
            return Index == other.Index &&
                   Value == other.Value &&
                   IsSelected == other.IsSelected;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Brainf_ckMemoryCell cell && Equals(cell);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Index;
                hashCode = (hashCode * 397) ^ Value;
                hashCode = (hashCode * 397) ^ Unsafe.As<bool, byte>(ref Unsafe.AsRef(in _IsSelected));

                return hashCode;
            }
        }
    }
}