using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            protected set => Set(ref _Source, value);
        }
    }
}
