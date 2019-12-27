using System.Collections.ObjectModel;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight;

namespace Brainf_ckSharp.UWP.ViewModels.Abstract
{
    /// <summary>
    /// A view model for a view that exposes a grouped collection of items of a given type
    /// </summary>
    public abstract class GroupedItemsCollectionViewModelBase<TKey, TValue> : ViewModelBase
    {
        private ObservableCollection<ObservableGroup<TKey, TValue>> _Source;

        /// <summary>
        /// Gets the source grouped collection for the current instance
        /// </summary>
        public ObservableCollection<ObservableGroup<TKey, TValue>> Source
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

        /// <inheritdoc cref="ViewModelBase.Cleanup"/>
        public override void Cleanup()
        {
            Source?.Clear();
            Source = null;
            View = null;
            CollectionGroups = null;
        }
    }
}
