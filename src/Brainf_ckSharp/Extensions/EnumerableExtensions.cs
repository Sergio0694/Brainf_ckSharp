using System;
using System.Diagnostics.Contracts;

namespace Brainf_ckSharp.Extensions
{
    /// <summary>
    /// A <see langword="class"/> with a collection of extension methods for enumerable types
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Adds an input item to a given <see cref="ReadOnlyMemory{T}"/> and returns a new <see cref="ReadOnlyMemory{T}"/> instance
        /// </summary>
        /// <typeparam name="T">The type of items in the input collection</typeparam>
        /// <param name="source">The input <see cref="ReadOnlyMemory{T}"/> instance to read values from</param>
        /// <param name="item">The new <typeparamref name="T"/> item to concat to the input <see cref="ReadOnlyMemory{T}"/> instance</param>
        /// <returns>A new <see cref="ReadOnlyMemory{T}"/> instance with all the original items, plus <paramref name="item"/></returns>
        [Pure]
        public static ReadOnlyMemory<T> Concat<T>(this ReadOnlyMemory<T> source, T item)
        {
            // If the input collection is empty, just create a new array
            if (source.Length == 0) return new[] { item };

            // Create a new array and copy the first items plus the one to concat
            T[] items = new T[source.Length + 1];
            source.CopyTo(items);
            items[source.Length] = item;

            return items;
        }
    }
}
