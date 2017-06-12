using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels
{
    /// <summary>
    /// A class that represents a single group to be displayed in a semantic zoom control
    /// </summary>
    /// <typeparam name="TKey">The type of the group key</typeparam>
    /// <typeparam name="TItems">The type of the items in the group</typeparam>
    public class JumpListGroup<TKey, TItems> : List<TItems>
    {
        public JumpListGroup([NotNull] TKey key, [CanBeNull] IEnumerable<TItems> collection) : base(collection ?? new List<TItems>())
        {
            Key = key;
        }

        public JumpListGroup([NotNull] IGrouping<TKey, TItems> collection) : base(collection)
        {
            Key = collection.Key;
        }

        /// <summary>
        /// Key that represents the group of objects and used as group header.
        /// </summary>
        public TKey Key { get; }
    }
}
