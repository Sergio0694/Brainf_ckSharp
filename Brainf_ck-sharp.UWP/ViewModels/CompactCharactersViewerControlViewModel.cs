using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class CompactCharactersViewerControlViewModel : ItemsCollectionViewModelBase<CharactersChunkModel>
    {
        public CompactCharactersViewerControlViewModel()
        {
            Messenger.Default.Register<ConsoleMemoryStateChangedMessage>(this, m => RefreshSourceAsync(m.State).Forget());
            RefreshSourceAsync(TouringMachineStateProvider.Initialize(AppSettingsParser.InterpreterMemorySize)).Forget();
        }

        // Refreshes the compact memory view
        private async Task RefreshSourceAsync([NotNull] IReadonlyTouringMachineState state)
        {
            List<CharactersChunkModel> data = await Task.Run(() =>
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
