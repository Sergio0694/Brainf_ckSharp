using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class UnicodeCharactersGuideFlyoutViewModel : ItemsCollectionViewModelBase<int>
    {
        private ObservableCollection<int> _SecondSource = new ObservableCollection<int>();

        /// <summary>
        /// Gets the second items collection for the current instance
        /// </summary>
        [NotNull]
        public ObservableCollection<int> SecondSource
        {
            get => _SecondSource;
            private set => Set(ref _SecondSource, value);
        }

        /// <summary>
        /// Initializes the list of characters to display
        /// </summary>
        public async Task LoadAsync()
        {
            (IEnumerable<int> first, IEnumerable<int> second) = await Task.Run(() => (Enumerable.Range(32, 96).ToArray(), Enumerable.Range(160, 96).ToArray()));
            Source = new ObservableCollection<int>(first);
            SecondSource = new ObservableCollection<int>(second);
        }
    }
}
