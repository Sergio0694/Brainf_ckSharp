using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels.Abstract
{
    /// <summary>
    /// An abstract class to be used inside a ViewModel for a page with a JumpList
    /// </summary>
    public abstract class JumpListViewModelBase<TKey, TValue> : ViewModelBase, IDisposable
    {
        public virtual void Dispose()
        {
            LoadingCompleted = null;
            Source?.Clear();
            Source = null;
            View = null;
            CollectionGroups = null;
        }

        /// <summary>
        /// Raised whenever the source is loaded, the argument is true if at least one item is present
        /// </summary>
        public event EventHandler<bool> LoadingCompleted;

        private IList<JumpListGroup<TKey, TValue>> _Source;

        /// <summary>
        /// Gets the user-defined items collection for this instance
        /// </summary>
        public IList<JumpListGroup<TKey, TValue>> Source
        {
            get => _Source;
            private set
            {
                if (_Source != value)
                {
                    // Save the current source
                    _Source = value;

                    // Create the collection source
                    if (value == null) return;
                    CollectionViewSource source = new CollectionViewSource
                    {
                        IsSourceGrouped = true,
                        Source = value
                    };

                    // Assign the source data
                    View = source.View;
                    CollectionGroups = source.View.CollectionGroups;
                }
            }
        }

        private ICollectionView _View;

        /// <summary>
        /// Gets the list of grouped items to display
        /// </summary>
        public ICollectionView View
        {
            get => _View;
            private set => Set(ref _View, value);
        }

        private IObservableVector<object> _CollectionGroups;

        /// <summary>
        /// Gets the list of group headers in the list
        /// </summary>
        public IObservableVector<object> CollectionGroups
        {
            get => _CollectionGroups;
            private set => Set(ref _CollectionGroups, value);
        }

        /// <summary>
        /// Loads the grouped items from the database
        /// </summary>
        public async Task LoadGroupsAsync()
        {
            IList<JumpListGroup<TKey, TValue>> source = await OnLoadGroupsAsync();
            LoadingCompleted?.Invoke(this, source.Count > 0);
            Source = source;
        }

        /// <summary>
        /// Performs the loading operation for the current instance
        /// </summary>
        [ItemNotNull]
        protected abstract Task<IList<JumpListGroup<TKey, TValue>>> OnLoadGroupsAsync();
    }
}
