using System;
using System.Threading.Tasks;
using Windows.Storage;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Messages.Ide;
using Brainf_ckSharp.Uwp.Messages.InputPanel;
using Brainf_ckSharp.Uwp.Services.Files;
using Brainf_ckSharp.Uwp.ViewModels.Abstract;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Views
{
    /// <summary>
    /// A view model for a Brainf*ck/PBrain IDE
    /// </summary>
    public sealed class IdeViewModel : ReactiveViewModelBase
    {
        /// <summary>
        /// Raised whenever a new character is requested to be added to the current text
        /// </summary>
        public event EventHandler<char>? CharacterAdded;

        /// <summary>
        /// Raised whenever a character is requested to be deleted
        /// </summary>
        public event EventHandler? CharacterDeleted;

        private string _LoadedText = "\r";

        /// <summary>
        /// Gets or sets the loaded source code
        /// </summary>
        public string LoadedText
        {
            get => _LoadedText;
            set => Set(ref _LoadedText, value);
        }

        /// <inheritdoc/>
        protected override void OnActivate()
        {
            Messenger.Default.Register<OperatorKeyPressedNotificationMessage>(this, m => CharacterAdded?.Invoke(this, m));
            Messenger.Default.Register<InsertNewLineRequestMessage>(this, _ => CharacterAdded?.Invoke(this, Characters.CarriageReturn));
            Messenger.Default.Register<DeleteCharacterRequestMessage>(this, _ => CharacterDeleted?.Invoke(this, EventArgs.Empty));
            Messenger.Default.Register<OpenFileRequestMessage>(this, m => _ = TryLoadTextFromFileAsync());
        }

        /// <summary>
        /// Tries to open and load a source code file
        /// </summary>
        private async Task TryLoadTextFromFileAsync()
        {
            if (!(await SimpleIoc.Default.GetInstance<IFilesService>().TryPickOpenFileAsync(".bfs") is StorageFile file)) return;

            LoadedText = await FileIO.ReadTextAsync(file);
        }
    }
}
