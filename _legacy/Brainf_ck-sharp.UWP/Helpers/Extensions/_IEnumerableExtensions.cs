using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.Extensions
{
    /// <summary>
    /// An extension class with helper methods for enumerable types
    /// </summary>
    public static class _IEnumerableExtensions
    {
        /// <summary>
        /// Inserts an item in the right position inside a list
        /// </summary>
        /// <typeparam name="TValue">The Type of the item to insert into the list</typeparam>
        /// <param name="list">The list to edit</param>
        /// <param name="newItem">The item to add</param>
        /// <param name="selectors">The functions that return a comparable value from an item</param>
        public static void AddSorted<TValue>(
            [NotNull] this IList<TValue> list, [NotNull] TValue newItem,
            [NotNull] params Func<TValue, IComparable>[] selectors) where TValue : class
        {
            // Input check
            if (selectors.Length == 0) throw new ArgumentOutOfRangeException(nameof(selectors), "The function needs at least a selector");

            // Clone the source list and insert the new item
            List<TValue> temp = new List<TValue> { newItem };
            temp.AddRange(list);

            // Sort the temporary list
            IOrderedEnumerable<TValue> query = temp.OrderBy(selectors[0]);
            if (selectors.Length > 1)
            {
                for (int i = 1; i < selectors.Length; i++) query = query.ThenBy(selectors[i]);
            }

            // Insert the new item in the original list in the right position
            list.Insert(query.IndexOf(item => item == newItem), newItem);
        }

        /// <summary>
        /// Makes sure the input list has its elements in the right order
        /// </summary>
        /// <typeparam name="TValue">The Type of the items in the list</typeparam>
        /// <param name="list">The list to edit</param>
        /// <param name="editedItem">The item that has just been edited</param>
        /// <param name="selectors">The functions that return a comparable value from an item</param>
        /// <returns>A value that indicates whether or not the input list has been changed</returns>
        public static bool EnsureSorted<TValue>(
            [NotNull] this IList<TValue> list, [NotNull] TValue editedItem,
            [NotNull] params Func<TValue, IComparable>[] selectors) where TValue : class
        {
            // Input check
            if (selectors.Length == 0) throw new ArgumentOutOfRangeException(nameof(selectors), "The function needs at least a selector");
            if (!list.Contains(editedItem)) throw new InvalidOperationException("The source list doesn't contain the given item");

            // Sort the temporary list
            IOrderedEnumerable<TValue> query = list.OrderBy(selectors[0]);
            if (selectors.Length > 1) for (int i = 1; i < selectors.Length; i++) query = query.ThenBy(selectors[i]);
            List<TValue> sorted = query.ToList();

            // Make sure the edited item is sorted correctly
            int index = sorted.IndexOf(editedItem);
            bool reordered = false;
            if (list.IndexOf(editedItem) != index)
            {
                list.Remove(editedItem);
                list.Insert(index, editedItem);
                reordered = true;
            }
            return reordered;
        }

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
    }
}
