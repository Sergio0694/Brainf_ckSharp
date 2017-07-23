using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels.Abstract.JumpList
{
    /// <summary>
    /// An abstract class to be used inside a ViewModel for a page with a JumpList, loading the data asynchronously
    /// </summary>
    public abstract class DeferredJumpListViewModelBase<TKey, TValue> : JumpListViewModelBase<TKey, TValue>
    {
        /// <inheritdoc cref="GalaSoft.MvvmLight.ViewModelBase.Cleanup"/>
        public override void Cleanup()
        {
            LoadingCompleted = null;
            base.Cleanup();
        }

        /// <summary>
        /// Raised whenever the source is loaded, the argument is true if at least one item is present
        /// </summary>
        public event EventHandler<bool> LoadingCompleted;

        /// <summary>
        /// Loads the grouped items from the database
        /// </summary>
        public async Task LoadGroupsAsync()
        {
            IList<JumpListGroup<TKey, TValue>> source = await OnLoadGroupsAsync();
            Source = new ObservableCollection<JumpListGroup<TKey, TValue>>(source);
            LoadingCompleted?.Invoke(this, source.Count > 0);
        }

        /// <summary>
        /// Performs the loading operation for the current instance
        /// </summary>
        [ItemNotNull]
        protected abstract Task<IList<JumpListGroup<TKey, TValue>>> OnLoadGroupsAsync();
    }
}
