using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Settings;
using Brainf_ck_sharp.Legacy.UWP.Messages.Actions;
using Brainf_ck_sharp.Legacy.UWP.ViewModels.Abstract;
using Brainf_ckSharp.Legacy.MemoryState;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.ViewModels
{
    public class CompactCharactersViewerControlViewModel : ItemsCollectionViewModelBase<CharactersChunkModel>
    {
        public CompactCharactersViewerControlViewModel()
        {
            Messenger.Default.Register<ConsoleMemoryStateChangedMessage>(this, m => RefreshSourceAsync(m.Value).Forget());
            RefreshSourceAsync(TouringMachineStateProvider.Initialize(AppSettingsParser.InterpreterMemorySize)).Forget();
        }

        // Refreshes the compact memory view
        private async Task RefreshSourceAsync([NotNull] IReadonlyTouringMachineState state)
        {
            IReadOnlyList<CharactersChunkModel> data = await Task.Run(() =>
            {
                List<CharactersChunkModel> temp = new List<CharactersChunkModel>();
                for (int i = 0; i < state.Count; i += 4)
                {
                    temp.Add(new CharactersChunkModel(new[] {state[i], state[i + 1], state[i + 2], state[i + 3]}, i));
                }
                return temp;
            });
            Source = new ObservableCollection<CharactersChunkModel>(data);
        }
    }
}
