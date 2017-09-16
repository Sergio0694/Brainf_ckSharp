using System.Collections.ObjectModel;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels;
using GalaSoft.MvvmLight;

namespace Brainf_ck_sharp_UWP.ViewModels.Abstract.JumpList
{
    /// <summary>
    /// An abstract class to be used inside a ViewModel for a page with a JumpList
    /// </summary>
    public abstract class JumpListViewModelBase<TKey, TValue> : ViewModelBase
    {
        /// <inheritdoc cref="ViewModelBase.Cleanup"/>
        public override void Cleanup()
        {
            Source?.Clear();
            Source = null;
            View = null;
            CollectionGroups = null;
        }

        private ObservableCollection<JumpListGroup<TKey, TValue>> _Source;

        /// <summary>
        /// Gets the user-defined items collection for this instance
        /// </summary>
        public ObservableCollection<JumpListGroup<TKey, TValue>> Source
        {
            get => _Source;
            protected set
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
    }
}
