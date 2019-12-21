using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Brainf_ck_sharp_UWP.ViewModels.Abstract
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
        public ObservableCollection<T> Source
        {
            get => _Source;
            protected set
            {
                // Update the source and the IsEmpty property
                if (Set(ref _Source, value))
                {
                    RaisePropertyChanged(() => IsEmpty);
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
        protected bool Clear()
        {
            if (IsEmpty) return false;
            Source = new ObservableCollection<T>();
            return true;
        }

        /// <inheritdoc cref="ViewModelBase.Cleanup"/>
        public override void Cleanup()
        {
            base.Cleanup();
            Source.Clear();
            Source = null;
        }
    }
}
