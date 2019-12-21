using System.Collections.Generic;
using System.Collections.ObjectModel;
using Brainf_ck_sharp.Legacy.UWP.DataModels;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.ViewModels.Abstract.JumpList
{
    /// <summary>
    /// An abstract class to be used inside a ViewModel for a page with a JumpList, loading the data synchronously
    /// </summary>
    public abstract class SyncJumpListViewModelBase<TKey, TValue> : JumpListViewModelBase<TKey, TValue>
    {
        /// <summary>
        /// Loads the grouped items from the database
        /// </summary>
        public void LoadGroups()
        {
            IList<JumpListGroup<TKey, TValue>> source = OnLoadGroups();
            Source = new ObservableCollection<JumpListGroup<TKey, TValue>>(source);
        }

        /// <summary>
        /// Performs the loading operation for the current instance
        /// </summary>
        [NotNull]
        protected abstract IList<JumpListGroup<TKey, TValue>> OnLoadGroups();
    }
}
