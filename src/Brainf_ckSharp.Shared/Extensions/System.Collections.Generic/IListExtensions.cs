using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;

namespace Brainf_ckSharp.Shared.Extensions.System.Collections.Generic;

/// <summary>
/// An extension <see langword="class"/> for <see cref="IList{T}"/> types
/// </summary>
public static class IListExtensions
{
    /// <summary>
    /// Inserts an item into a target list in order to keep the list sorted
    /// </summary>
    /// <typeparam name="T">The type of items in the list</typeparam>
    /// <param name="list">The target list to modify</param>
    /// <param name="item">The <typeparamref name="T"/> item to insert into <paramref name="list"/></param>
    /// <param name="comparer">The comparer function to use to compare <typeparamref name="T"/> items</param>
    /// <remarks>The target item will always be the second parameter for <paramref name="comparer"/></remarks>
    public static void InsertSorted<T>(this IList<T> list, T item, Func<T, T, int> comparer)
    {
        // Empty list
        if (list.Count == 0)
        {
            list.Add(item);

            return;
        }

        // The item should be added in first position
        if (comparer(list[0], item) <= 0)
        {
            list.Insert(0, item);

            return;
        }

        // The item should be added in last position
        if (comparer(list[list.Count - 1], item) >= 0)
        {
            list.Add(item);

            return;
        }

        // Find the right place to insert the new item
        for (int i = 1; i < list.Count; i++)
        {
            if (comparer(list[i], item) <= 0)
            {
                list.Insert(i, item);

                return;
            }
        }

        ThrowHelper.ThrowInvalidOperationException("Error inserting the input item");
    }
}
