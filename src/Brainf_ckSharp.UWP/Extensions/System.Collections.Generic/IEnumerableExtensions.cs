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
