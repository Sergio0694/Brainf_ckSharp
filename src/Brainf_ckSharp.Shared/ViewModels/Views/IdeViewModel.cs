using System;
using System.Threading.Tasks;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Messages.Ide;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.Models.Ide;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Views
{
    /// <summary>
    /// A view model for a Brainf*ck/PBrain IDE
    /// </summary>
    public sealed class IdeViewModel : ViewModelBase
    {
        /// <summary>
        /// Raised whenever a script is requested to be run
        /// </summary>
        public event EventHandler? ScriptRunRequested;

        /// <summary>
        /// Raised whenever a script is requested to be debugged
        /// </summary>
        public event EventHandler? ScriptDebugRequested;

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
                }
            }
        }

        private string _CurrentText = SourceCode.EmptyContent;

        /// <summary>
        /// Gets or sets the currently displayed text
        /// </summary>
        public string CurrentText
        {
            get => _CurrentText;
            set
            {
                if (Set(ref _CurrentText, value))
                {
                    IsUnsavedEditPending = value != Code.Content;
                }
            }
        }

        private bool _IsUnsavedEditPending;

        /// <summary>
        /// Gets whether or not there are pending unsaved changes to the current file
        /// </summary>
        public bool IsUnsavedEditPending
        {
            get => _IsUnsavedEditPending;
            private set => Set(ref _IsUnsavedEditPending, value);
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            Messenger.Register<OperatorKeyPressedNotificationMessage>(this, m => CharacterAdded?.Invoke(this, m.Value));
            Messenger.Register<RunIdeScriptRequestMessage>(this, _ => ScriptRunRequested?.Invoke(this, EventArgs.Empty));
            Messenger.Register<DebugIdeScriptRequestMessage>(this, _ => ScriptDebugRequested?.Invoke(this, EventArgs.Empty));
            Messenger.Register<InsertNewLineRequestMessage>(this, _ => CharacterAdded?.Invoke(this, Characters.CarriageReturn));
            Messenger.Register<DeleteCharacterRequestMessage>(this, _ => CharacterDeleted?.Invoke(this, EventArgs.Empty));
            Messenger.Register<PickOpenFileRequestMessage>(this, m => _ = TryLoadTextFromFileAsync(m.Favorite));
            Messenger.Register<LoadSourceCodeRequestMessage>(this, m => Code = m.Value);
            Messenger.Register<SaveFileRequestMessage>(this, m => _ = TrySaveTextAsync());
            Messenger.Register<SaveFileAsRequestMessage>(this, m => _ = TrySaveTextAsAsync());
        }

        /// <summary>
        /// Tries to open and load a source code file
        /// </summary>
        /// <param name="favorite">Whether to immediately mark the item as favorite</param>
        private async Task TryLoadTextFromFileAsync(bool favorite)
        {
            if (!(await Ioc.Default.GetRequiredService<IFilesService>().TryPickOpenFileAsync(".bfs") is IFile file)) return;

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
            IFilesService filesService = Ioc.Default.GetRequiredService<IFilesService>();

            if (!(await filesService.TryPickSaveFileAsync(string.Empty, (string.Empty, ".bfs")) is IFile file)) return;

            await Code.TrySaveAsAsync(file);

            CodeSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}
