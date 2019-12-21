using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;
using Brainf_ckSharp.Legacy.MemoryState;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class MemoryViewerFlyoutViewModel : ItemsCollectionViewModelBase<IndexedModelWithValue<Brainf_ckMemoryCell>>
    {
        /// <summary>
        /// Initializes the indexed memory cells to display in the control
        /// </summary>
        /// <param name="state">The source memory state to load</param>
        public async Task InitializeAsync([NotNull] IReadonlyTouringMachineState state)
        {
            Source = await Task.Run(() =>
            {
                IndexedModelWithValue<Brainf_ckMemoryCell>[] indexed = IndexedModelWithValue<Brainf_ckMemoryCell>.New(state).ToArray();
                return new ObservableCollection<IndexedModelWithValue<Brainf_ckMemoryCell>>(indexed);
            });
            InitializationCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised whenever the initialization process completes
        /// </summary>
        public event EventHandler InitializationCompleted;

        /// <inheritdoc/>
        public override void Cleanup()
        {
            base.Cleanup();
            InitializationCompleted = null;
        }
    }
}
