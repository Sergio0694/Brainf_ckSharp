using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.Extensions
{
    /// <summary>
    /// A simple static class with some useful extension methods
    /// </summary>
    public static class MiscExtensions
    {
        /// <summary>
        /// Performs a direct cast on the given object to a specific type
        /// </summary>
        /// <typeparam name="T">The tye to return</typeparam>
        /// <param name="o">The object to cast</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>([CanBeNull] this object o) => (T)o;

        /// <summary>
        /// Suppresses the warnings when calling an async method without awaiting it
        /// </summary>
        /// <param name="task">The Task returned by the async call</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task) { }

        /// <summary>
        /// Suppresses the warnings when calling an async method without awaiting it
        /// </summary>
        /// <param name="action">The IAsyncAction returned by the async call</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this IAsyncAction action) { }

        /// <summary>
        /// Performs a loop on the given collection, calling the input delegate for each item
        /// </summary>
        /// <typeparam name="T">The Type to cast the collection items to</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="action">The delegate to call for each item in the collection</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TypedForEach<T>([NotNull] this IEnumerable source, [NotNull] Action<T> action)
        {
            foreach (object item in source) action(item.To<T>());
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable</summary>
        ///<param name="items">The enumerable to search</param>
        ///<param name="predicate">The expression to test the items against</param>
        ///<returns>The index of the first matching item, or -1 if no items match</returns>
        public static int IndexOf<T>([NotNull] this IEnumerable<T> items, [NotNull] Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T item in items)
            {
                if (predicate(item)) return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Finds the coordinates in a multiline string for the given index
        /// </summary>
        /// <param name="text">The input text</param>
        /// <param name="index">The target text index</param>
        /// <param name="newline">The newline character to use</param>
        public static (int Y, int X) FindCoordinates([NotNull] this String text, int index, char newline = '\r')
        {
            int
                row = 1,
                col = 1;
            for (int i = 0; i < index; i++)
            {
                if (text[i] == '\r')
                {
                    row++;
                    col = 1;
                }
                else col++;
            }
            return (row, col);
        }
    }
}
