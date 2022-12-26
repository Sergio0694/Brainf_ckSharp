using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Brainf_ckSharp.Uwp.Extensions.System.Collections.Generic;

/// <summary>
/// An extension <see langword="class"/> for <see cref="IReadOnlyList{T}"/> types
/// </summary>
public static class IReadOnlyListExtensions
{
    /// <summary>
    /// An implementation of <see cref="Enumerable.Reverse{T}(IEnumerable{T})"/> that avoids the initial enumeration
    /// </summary>
    /// <typeparam name="T">The type of items in the input list</typeparam>
    /// <param name="items">The input list of <typeparamref name="T"/> items</param>
    /// <returns>An enumeration of the input items in reverse order</returns>
    [Pure]
    public static IEnumerable<T> Reverse<T>(this IReadOnlyList<T> items)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            yield return items[i];
        }
    }
}
