using System;

namespace Brainf_ckSharp.Models
{
    /// <summary>
    /// A <see langword="class"/> that contains the info on a function definition in a script
    /// </summary>
    public sealed class FunctionDefinition : IEquatable<FunctionDefinition>
    {
        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="value">The function value</param>
        /// <param name="index">The numerical index for the current function definition</param>
        /// <param name="offset">The function script offset</param>
        /// <param name="body">The function code</param>
        public FunctionDefinition(ushort value, int index, int offset, string body)
        {
            Value = value;
            Index = index;
            Offset = offset;
            Body = body;
        }

        /// <summary>
        /// Gets the numeric value associated with the function definition
        /// </summary>
        public ushort Value { get; }

        /// <summary>
        /// Gets the numerical index for the current function definition
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the offset of the function in the original source code
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the source code of the defined function
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Checks whether or not two <see cref="FunctionDefinition"/> instances are equal
        /// </summary>
        /// <param name="a">The first <see cref="FunctionDefinition"/> instance to compare</param>
        /// <param name="b">The second <see cref="FunctionDefinition"/> instance to compare</param>
        /// <returns><see langword="true"/> if the two input <see cref="FunctionDefinition"/> are equal, <see langword="false"/> otherwise</returns>
        public static bool operator ==(FunctionDefinition? a, FunctionDefinition? b) => a?.Equals(b) == true;

        /// <summary>
        /// Checks whether or not two <see cref="FunctionDefinition"/> instances are not equal
        /// </summary>
        /// <param name="a">The first <see cref="FunctionDefinition"/> instance to compare</param>
        /// <param name="b">The second <see cref="FunctionDefinition"/> instance to compare</param>
        /// <returns><see langword="true"/> if the two input <see cref="FunctionDefinition"/> are not equal, <see langword="false"/> otherwise</returns>
        public static bool operator !=(FunctionDefinition? a, FunctionDefinition? b) => a?.Equals(b) != true;

        /// <inheritdoc/>
        public bool Equals(FunctionDefinition? other)
        {
            if (other is null) return false;

            return
                Value == other.Value &&
                Index == other.Index &&
                Offset == other.Offset &&
                Body.Equals(other.Body);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as FunctionDefinition);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Index, Offset, Body);
        }
    }
}
