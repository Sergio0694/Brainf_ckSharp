using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// An extension <see langword="class"/> for <see cref="IList{T}"/> types
    /// </summary>
    internal static class IListExtensions
    {
        /// <summary>
        /// An implementation of <see cref="Enumerable.LastOrDefault{T}(IEnumerable{T})"/> that works in O(1) on <see cref="IList{T}"/> instances
        /// </summary>
        /// <typeparam name="T">The type of items in the input list</typeparam>
        /// <param name="items">The input list of <typeparamref name="T"/> items</param>
        /// <returns>Either the last item in <paramref name="items"/>, or the default <typeparamref name="T"/> value</returns>
        [Pure]
        public static T LastOrDefault<T>(this IList<T> items)
        {
            return items.Count switch
            {
                0 => default,
                { } n => items[n - 1]
            };
        }

        /// <summary>
        /// Removes the last item in a given <see cref="IList{T}"/> instance
        /// </summary>
        /// <typeparam name="T">The type of items in the input list</typeparam>
        /// <param name="items">The input list of <typeparamref name="T"/> items</param>
        public static void RemoveLast<T>(this IList<T> items) => items.RemoveAt(items.Count - 1);
    }
}
