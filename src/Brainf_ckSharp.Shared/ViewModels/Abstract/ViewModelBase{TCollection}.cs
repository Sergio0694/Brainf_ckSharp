using System.Collections;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace Brainf_ckSharp.Shared.ViewModels.Abstract
{
    /// <summary>
    /// A view model for a view that exposes a collection of a given type
    /// </summary>
    /// <typeparam name="TCollection">The type of collection to use</typeparam>
    public abstract class ViewModelBase<TCollection> : ObservableRecipient
        where TCollection : class, IList, new()
    {
        private TCollection _Source = new TCollection();

        /// <summary>
        /// Gets the items collection for the current instance
        /// </summary>
        public TCollection Source
        {
            get => _Source;
            protected set
            {
                // Update the source and the dependent properties
                if (SetProperty(ref _Source, value))
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
