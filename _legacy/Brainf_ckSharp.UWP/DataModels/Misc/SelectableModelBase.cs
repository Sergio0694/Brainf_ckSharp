using GalaSoft.MvvmLight;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc
{
    /// <summary>
    /// A base class for a selectable model that contains an item of a given type
    /// </summary>
    public abstract class SelectableModelBase<T> : ViewModelBase
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
            set => Set(ref _IsSelected, value);
        }

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
