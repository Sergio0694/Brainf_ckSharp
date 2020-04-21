using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace Brainf_ckSharp.Shared.ViewModels.Abstract
{
    /// <summary>
    /// A view model for a view that exposes a collection of items of a given type
    /// </summary>
    /// <typeparam name="T">The type of items in the exposed collection</typeparam>
    public abstract class ItemsCollectionViewModelBase<T> : ViewModelBase
    {
        private ObservableCollection<T> _Source = new ObservableCollection<T>();

        /// <summary>
        /// Gets the items collection for the current instance
        /// </summary>
        public ObservableCollection<T> Source
        {
            get => _Source;
            protected set
            {
                // Update the source and the dependent properties
                if (Set(ref _Source, value))
                {
                    OnPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        /// <summary>
        /// Gets whether or not the current source collection is empty
        /// </summary>
        public bool IsEmpty => Source.Count == 0;

        /// <summary>
        /// Clears the collection of items, if possible
        /// </summary>
        /// <returns><see langword="true"/> if the collection was cleared, <see langword="false"/> if it was already empty</returns>
        protected bool TryClearSource()
        {
            if (IsEmpty) return false;

            Source.Clear();
            return true;
        }
    }
}
