using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// A small <see langword="class"/> with some static extensions for an <see langword="enum"/> of a specific type
    /// </summary>
    /// <typeparam name="T">The type of <see langword="enum"/> for the current instance</typeparam>
    public static class Enum<T> where T : struct, Enum
    {
        /// <summary>
        /// Gets the list of values for the current instance
        /// </summary>
        public static ReadOnlyMemory<T> Values { get; } = (T[])Enum.GetValues(typeof(T));
    }
}
