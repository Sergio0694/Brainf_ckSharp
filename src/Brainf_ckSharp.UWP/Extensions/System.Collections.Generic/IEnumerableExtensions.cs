using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// An extension <see langword="class"/> for <see cref="IEnumerable{T}"/> types
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> sequence with just a given <typeparamref name="T"/> element
        /// </summary>
        /// <typeparam name="T">The type of the input element</typeparam>
        /// <param name="item">The input element for the sequence</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence that contains <paramref name="item"/></returns>
        [Pure]
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Enumerates the input sequence of <typeparamref name="T"/> items with their index
        /// </summary>
        /// <typeparam name="T">The type of items in the input sequence</typeparam>
        /// <param name="items">The input sequence of <typeparamref name="T"/> items</param>
        /// <returns>A sequence of pairs of items and indices</returns>
        [Pure]
        public static IEnumerable<(T Value, int Index)> Enumerate<T>(this IEnumerable<T> items)
        {
            return items.Select((item, index) => (item, index));
        }
    }
}
