using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels
{
    /// <summary>
    /// A class that represents a single group to be displayed in a semantic zoom control
    /// </summary>
    /// <typeparam name="TKey">The type of the group key</typeparam>
    /// <typeparam name="TItems">The type of the items in the group</typeparam>
    public class JumpListGroup<TKey, TItems> : ObservableCollection<TItems>
    {
        public JumpListGroup([NotNull] TKey key, [CanBeNull] IEnumerable<TItems> collection) : base(collection ?? new List<TItems>())
        {
            Key = key;
        }

        public JumpListGroup([NotNull] IGrouping<TKey, TItems> group) : base(group)
        {
            Key = group.Key;
        }

        /// <summary>
        /// Key that represents the group of objects and used as group header.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Gets the first item in the group (the only one if used for a single-item list)
        /// </summary>
        public TItems Sample => this.FirstOrDefault();
    }
}
