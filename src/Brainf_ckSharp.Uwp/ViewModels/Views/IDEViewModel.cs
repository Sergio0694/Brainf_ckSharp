using System;
using System.Threading.Tasks;
using Windows.Storage;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Messages.Ide;
using Brainf_ckSharp.Uwp.Messages.InputPanel;
using Brainf_ckSharp.Uwp.Messages.Navigation;
using Brainf_ckSharp.Uwp.Models.Ide;
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
        /// Raised whenever a script is requested to be run
        /// </summary>
        public event EventHandler ScriptRunRequested;

        /// <summary>
        /// Raised whenever a new character is requested to be added to the current text
        /// </summary>
        public event EventHandler<char>? CharacterAdded;

        /// <summary>
        /// Raised whenever a character is requested to be deleted
        /// </summary>
        public event EventHandler? CharacterDeleted;

        /// <summary>
        /// Raised whenever a new source code is loaded and used as a reference
        /// </summary>
        public event EventHandler<string>? CodeLoaded;

        /// <summary>
        /// Raised whenever the current source code is saved by the user
        /// </summary>
        public event EventHandler? CodeSaved;

        private SourceCode _Code = SourceCode.CreateEmpty();

        /// <summary>
        /// Gets or sets the loaded source code
        /// </summary>
        public SourceCode Code
        {
            get => _Code;
            private set
            {
                if (Set(ref _Code, value))
                {
                    CodeLoaded?.Invoke(this, value.Content);

                    Messenger.Default.Send<SubPageCloseRequestMessage>();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnActivate()
        {
            Messenger.Default.Register<OperatorKeyPressedNotificationMessage>(this, m => CharacterAdded?.Invoke(this, m));
            Messenger.Default.Register<RunIdeScriptRequestMessage>(this, _ => ScriptRunRequested?.Invoke(this, EventArgs.Empty));
            Messenger.Default.Register<InsertNewLineRequestMessage>(this, _ => CharacterAdded?.Invoke(this, Characters.CarriageReturn));
            Messenger.Default.Register<DeleteCharacterRequestMessage>(this, _ => CharacterDeleted?.Invoke(this, EventArgs.Empty));
            Messenger.Default.Register<PickOpenFileRequestMessage>(this, m => _ = TryLoadTextFromFileAsync(m.Favorite));
            Messenger.Default.Register<LoadSourceCodeRequestMessage>(this, m => Code = m);
            Messenger.Default.Register<SaveFileRequestMessage>(this, m => _ = TrySaveTextAsync());
            Messenger.Default.Register<SaveFileAsRequestMessage>(this, m => _ = TrySaveTextAsAsync());
        }

        /// <summary>
        /// Tries to open and load a source code file
        /// </summary>
        /// <param name="favorite">Whether to immediately mark the item as favorite</param>
        private async Task TryLoadTextFromFileAsync(bool favorite)
        {
            if (!(await SimpleIoc.Default.GetInstance<IFilesService>().TryPickOpenFileAsync(".bfs") is StorageFile file)) return;

            if (await SourceCode.TryLoadFromEditableFileAsync(file) is SourceCode code)
            {
                Code = code;

                // Set the favorite state, if requested
                if (favorite)
                {
                    Code.Metadata.IsFavorited = true;

                    await Code.TrySaveAsync();
                }
            }
        }

        /// <summary>
        /// Tries to save the current text to the current file, if possible
        /// </summary>
        private async Task TrySaveTextAsync()
        {
            if (Code.File == null) await TrySaveTextAsAsync();
            else await Code.TrySaveAsync();

            CodeSaved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tries to save the current text to a new file
        /// </summary>
        private async Task TrySaveTextAsAsync()
        {
            IFilesService filesService = SimpleIoc.Default.GetInstance<IFilesService>();

            if (!(await filesService.TryPickSaveFileAsync(string.Empty, (string.Empty, ".bfs")) is StorageFile file)) return;

            await Code.TrySaveAsAsync(file);

            CodeSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}
