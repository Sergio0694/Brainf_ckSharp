using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Brainf_ckSharp.Uwp.Extensions.System.Collections.Generic;

/// <summary>
/// An extension <see langword="class"/> for <see cref="IList{T}"/> types
/// </summary>
public static class IListExtensions
{
    /// <summary>
    /// Removes the last item in a given <see cref="IList{T}"/> instance
    /// </summary>
    /// <typeparam name="T">The type of items in the input list</typeparam>
    /// <param name="items">The input list of <typeparamref name="T"/> items</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveLast<T>(this IList<T> items)
    {
        items.RemoveAt(items.Count - 1);
    }
}
