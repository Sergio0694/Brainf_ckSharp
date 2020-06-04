using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Messages.Ide;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.Models.Ide;
using Brainf_ckSharp.Shared.ViewModels.Views.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Views
{
    /// <summary>
    /// A view model for a Brainf*ck/PBrain IDE
    /// </summary>
    public sealed class IdeViewModel : WorkspaceViewModelBase
    {
        /// <summary>
        /// Creates a new <see cref="IdeViewModel"/> instance
        /// </summary>
        public IdeViewModel()
        {
            CodeSnippets = new[]
            {
                new CodeSnippet("Reset cell", "[-]"),
                new CodeSnippet("Duplicate value", "[>+>+<<-]>>[<<+>>-]<<"),
                new CodeSnippet("if (x == 0) then { }", ">+<[>-]>[->[-]]<<"),
                new CodeSnippet("if (x > 0) then { } else { }", ">+<[>[-]]>[->[-]]<<")
            };
        }

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

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            Messenger.Register<OperatorKeyPressedNotificationMessage>(this, m => CharacterAdded?.Invoke(this, m.Value));
            Messenger.Register<RunIdeScriptRequestMessage>(this, _ => ScriptRunRequested?.Invoke(this, EventArgs.Empty));
            Messenger.Register<DebugIdeScriptRequestMessage>(this, _ => ScriptDebugRequested?.Invoke(this, EventArgs.Empty));
            Messenger.Register<InsertNewLineRequestMessage>(this, _ => CharacterAdded?.Invoke(this, Characters.CarriageReturn));
            Messenger.Register<DeleteCharacterRequestMessage>(this, _ => CharacterDeleted?.Invoke(this, EventArgs.Empty));
            Messenger.Register<PickOpenFileRequestMessage>(this, m => _ = TryLoadTextFromFileAsync(m.Favorite));
            Messenger.Register<LoadSourceCodeRequestMessage>(this, m => LoadSourceCode(m.Value));
            Messenger.Register<SaveFileRequestMessage>(this, m => _ = TrySaveTextAsync());
            Messenger.Register<SaveFileAsRequestMessage>(this, m => _ = TrySaveTextAsAsync());
        }

        /// <summary>
        /// Gets the collection of available code snippets
        /// </summary>
        public IReadOnlyList<CodeSnippet> CodeSnippets { get; }

        /// <summary>
        /// Loads a specific <see cref="SourceCode"/> instance
        /// </summary>
        /// <param name="code">The source code to load</param>
        private void LoadSourceCode(SourceCode code)
        {
            Code = code;

            CodeLoaded?.Invoke(this, Code.Content);
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
                // Set the favorite state, if requested
                if (favorite)
                {
                    code.Metadata.IsFavorited = true;

                    await code.TrySaveAsync();
                }

                LoadSourceCode(code);
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
