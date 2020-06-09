using System;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc
{
    /// <summary>
    /// An empty class that represents the F# Unit type
    /// </summary>
    public sealed class Unit : IEquatable<Unit>, IComparable<Unit>
    {
        // Private constructor
        private Unit() { }

        /// <summary>
        /// Gets the default <see cref="Unit"/> value
        /// </summary>
        public static Unit Instance => null;

        /// <summary>
        /// Checks whether or not two <see cref="Unit"/> values are equal (they will always be)
        /// </summary>
        /// <param name="other">The value to compare</param>
        public bool Equals(Unit other) => true;

        /// <summary>
        /// Gets the default hash code for the current instance (it will always be 0)
        /// </summary>
        public override int GetHashCode() => 0;

        /// <summary>
        /// Compares two <see cref="Unit"/> values (they will always be equal)
        /// </summary>
        /// <param name="other">The value to compare</param>
        public int CompareTo(Unit other) => 0;
    }
}
