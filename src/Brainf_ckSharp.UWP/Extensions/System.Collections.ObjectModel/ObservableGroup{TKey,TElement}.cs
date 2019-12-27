using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A <see laangword="class"/> that represents a group of elements backed by an observable collection
    /// </summary>
    /// <typeparam name="TKey">The type of the group key</typeparam>
    /// <typeparam name="TElement">The type of the elements in the group</typeparam>
    public class ObservableGroup<TKey, TElement> : ObservableCollection<TElement>
    {
        /// <summary>
        /// Creates a new <see cref="ObservableGroup{TKey,TElement}"/> instance with the specified parameters
        /// </summary>
        /// <param name="key">The key for the new group</param>
        /// <param name="collection">The initial sequence of items in the group</param>
        public ObservableGroup(TKey key, IEnumerable<TElement> collection) : base(collection)
        {
            Key = key;
        }

        /// <summary>
        /// Creates a new <see cref="ObservableGroup{TKey,TElement}"/> instance with the specified parameters
        /// </summary>
        /// <param name="group">The source <see cref="IGrouping{TKey,TElement}"/> instance to read the initial data from</param>
        public ObservableGroup(IGrouping<TKey, TElement> group) : base(group)
        {
            Key = group.Key;
        }

        /// <summary>
        /// Gets the <typeparamref name="TKey"/> value that represents the current group
        /// </summary>
        public TKey Key { get; }
    }
}
