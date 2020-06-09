using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Collections;

namespace Brainf_ckSharp.Shared.Extensions.Microsoft.Toolkit.Collections
{
    /// <summary>
    /// An extension <see langword="class"/> for <see cref="ObservableCollection{T}"/> type
    /// </summary>
    public static class ObservableGroupedCollectionExtensions
    {
        /// <summary>
        /// Adds a key-value <see cref="ObservableGroup{TKey,TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey,TElement}"/>
        /// </summary>
        /// <typeparam name="TKey">The type of the group key</typeparam>
        /// <typeparam name="TElement">The type of the elements in the group</typeparam>
        /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey,TElement}"/> instance</param>
        /// <param name="key">The key to add</param>
        /// <param name="element">The element to add</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TKey, TElement>(
            this ObservableGroupedCollection<TKey, TElement> source,
            TKey key,
            TElement element)
            where TKey : notnull
        {
            Add(source, key, new [] { element });
        }

        /// <summary>
        /// Adds a key-collection <see cref="ObservableGroup{TKey,TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey,TElement}"/>
        /// </summary>
        /// <typeparam name="TKey">The type of the group key</typeparam>
        /// <typeparam name="TElement">The type of the elements in the group</typeparam>
        /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey,TElement}"/> instance</param>
        /// <param name="key">The key to add</param>
        /// <param name="collection">The collection to add</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<TKey, TElement>(
            this ObservableGroupedCollection<TKey, TElement> source,
            TKey key,
            IEnumerable<TElement> collection)
            where TKey : notnull
        {
            source.Add(new ObservableGroup<TKey, TElement>(key, collection));
        }
    }
}
