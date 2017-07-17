using System;
using GalaSoft.MvvmLight;

namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    /// <summary>
    /// A base class for a selectable model that contains an item of a given type
    /// </summary>
    public abstract class SelectableModelBase<T> : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Creates a new instance around the input item
        /// </summary>
        /// <param name="value">The item to wrap</param>
        protected SelectableModelBase(T value) => InnerValue = value;

        private bool _IsSelected;

        /// <summary>
        /// Gets or sets whether or not the current model is marked as selected
        /// </summary>
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (Set(ref _IsSelected, value))
                    IsSelectedPropertyChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Raised when the IsSelected property value is changed
        /// </summary>
        public event EventHandler<bool> IsSelectedPropertyChanged;

        /// <summary>
        /// Removes the current event handlers in this instance
        /// </summary>
        public void Dispose() => IsSelectedPropertyChanged = null;

        private T _InnerValue;

        /// <summary>
        /// The current value stored inside the model
        /// </summary>
        public T InnerValue
        {
            get => _InnerValue;
            set => Set(ref _InnerValue, value);
        }
    }
}
