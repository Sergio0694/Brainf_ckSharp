using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    /// <summary>
    /// A VM for a view that exposes a collection of items of the same type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemsCollectionViewModelBase<T> : ViewModelBase
    {
        private ObservableCollection<T> _Source = new ObservableCollection<T>();

        /// <summary>
        /// Gets the items collection for the current instance
        /// </summary>
        [NotNull]
        public ObservableCollection<T> Source
        {
            get => _Source;
            protected set
            {
                // Update the source and the IsEmpty property
                if (Set(ref _Source, value))
                {
                    IsEmpty = value.Count == 0;
                }
            }
        }

        private bool _IsEmpty;

        /// <summary>
        /// Gets whether or not the current source collection is empty
        /// </summary>
        public bool IsEmpty
        {
            get => _IsEmpty;
            private set => Set(ref _IsEmpty, value);
        }

        /// <summary>
        /// Clears the collection of items, if possible
        /// </summary>
        protected bool Clear()
        {
            if (IsEmpty) return false;
            Source = new ObservableCollection<T>();
            return true;
        }
    }
}
