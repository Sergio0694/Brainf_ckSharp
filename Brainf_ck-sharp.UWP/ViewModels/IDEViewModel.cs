using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Text;
using Brainf_ck_sharp;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.DataModels.Misc.IDEIndentationGuides;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Messages.IDEStatus;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class IDEViewModel : ItemsCollectionViewModelBase<IDEIndentationLineInfo>
    {
        // The current document that's linked to the view
        private readonly ITextDocument Document;

        // A UI-bound function that asks the user to pick a name to save a new source code
        private readonly Func<String, Task<String>> SaveNameSelector;

        // A function that retrieves the list of breakpoints currently present in the code
        private readonly Func<IReadOnlyCollection<int>> BreakpointsExtractor;

        /// <summary>
        /// Creates a new instance to manage the IDE
        /// </summary>
        /// <param name="document">The target document that contains the source code to edit</param>
        /// <param name="nameSelector">A function that prompts the user to enter a name to save a new source code in the app</param>
        /// <param name="breakpointsExtractor">A function that retrieves the active breakpoints</param>
        public IDEViewModel([NotNull] ITextDocument document, [NotNull] Func<String, 
            Task<String>> nameSelector, [NotNull] Func<IReadOnlyCollection<int>> breakpointsExtractor)
        {
            Document = document;
            SaveNameSelector = nameSelector;
            BreakpointsExtractor = breakpointsExtractor;
        }

        private bool _IsEnabled;

        /// <summary>
        /// Gets or sets whether or not the instance is enabled and it is processing incoming messages
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (Set(ref _IsEnabled, value))
                {
                    if (value)
                    {
                        Messenger.Default.Register<OperatorAddedMessage>(this, op => CharInsertionRequested?.Invoke(this, op.Operator));
                        Messenger.Default.Register<ClearScreenMessage>(this, m => TryClearScreen());
                        Messenger.Default.Register<PlayScriptMessage>(this, m => PlayRequested?.Invoke(this, (m.StdinBuffer, m.Type == ScriptPlayType.Debug)));
                        Messenger.Default.Register<SaveSourceCodeRequestMessage>(this, m => ManageSaveCodeRequest(m.RequestType).Forget());
                        Messenger.Default.Register<IDEUndoRedoRequestMessage>(this, m => ManageUndoRedoRequest(m.Operation));
                        Messenger.Default.Register<SourceCodeLoadingRequestedMessage>(this, m =>
                        {
                            _CategorizedCode = m.RequestedCode;
                            LoadedCodeChanged?.Invoke(this, m.RequestedCode.Code);
                            Messenger.Default.Send(new SaveButtonsEnabledStatusChangedMessage(m.RequestedCode.Type != SavedSourceCodeType.Sample, true));
                        });
                        Messenger.Default.Send(new SaveButtonsEnabledStatusChangedMessage(false, true)); // Default save buttons status
                        SendMessages();
                    }
                    else Messenger.Default.Unregister(this);
                }
            }
        }

        private CategorizedSourceCode _CategorizedCode;

        /// <summary>
        /// Gets the source code currently loaded, if present
        /// </summary>
        public SourceCode LoadedCode => _CategorizedCode?.Code;

        /// <summary>
        /// Raised whenever the current loaded source code changes
        /// </summary>
        public event EventHandler<SourceCode> LoadedCodeChanged; 

        /// <summary>
        /// Saves the current source code in the database
        /// </summary>
        /// <param name="type">The requested save operation</param>
        private async Task ManageSaveCodeRequest(CodeSaveType type)
        {
            // Get the text and the breakpoints
            Document.GetText(TextGetOptions.None, out String text);
            IReadOnlyCollection<int>
                raw = BreakpointsExtractor(),
                breakpoints = raw.Count > 0 ? raw : null;
            if (text[text.Length - 1] == '\r') text = text.Substring(0, text.Length - 1); // Remove final '\r' to avoid addednew lines

            // Save the file as requested
            switch (type)
            {
                // Save an already existing file
                case CodeSaveType.Save:
                    if (LoadedCode == null) throw new InvalidOperationException("There isn't a loaded source code to save");
                    await SQLiteManager.Instance.SaveCodeAsync(LoadedCode, text, breakpoints);
                    UpdateGitDiffStatusOnSave();
                    break;

                // Save the current code as a new file
                case CodeSaveType.SaveAs:
                    String name = await SaveNameSelector(text);
                    if (!String.IsNullOrEmpty(name))
                    {
                        AsyncOperationResult<CategorizedSourceCode> result = await SQLiteManager.Instance.SaveCodeAsync(name, text, breakpoints);
                        if (result)
                        {
                            // Update the local code reference, the git diff indicators and notify the UI with the new save buttons state
                            _CategorizedCode = result.Result;
                            NotificationsManager.ShowNotification(0xEC24.ToSegoeMDL2Icon(), LocalizationManager.GetResource("CodeSaved"),
                                LocalizationManager.GetResource("CodeSavedBody"), NotificationType.Default);
                            UpdateGitDiffStatusOnSave();
                            Messenger.Default.Send(new SaveButtonsEnabledStatusChangedMessage(true, true));
                            SendMessages(text);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Raised whenever the user requests to play the current script
        /// </summary>
        public event EventHandler<(String Stdin, bool Debug)> PlayRequested;

        // Indicates whether or not the IDE contains at least a valid operator
        private bool _CanExecute;

        /// <summary>
        /// Sends the status info messages for the current state
        /// </summary>
        public void SendMessages([CanBeNull] String code = null)
        {
            if (code == null) Document.GetText(TextGetOptions.None, out code);
            Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.ClearScreen, code.Length > 1));
            (bool valid, int error) = Brainf_ckInterpreter.CheckSourceSyntax(code);
            (int row, int col) = code.FindCoordinates(Document.Selection.StartPosition);
            bool executable = Brainf_ckInterpreter.FindOperators(code);
            if (_CanExecute != executable)
            {
                Messenger.Default.Send(new IDEExecutableStatusChangedMessage(executable, BreakpointsExtractor().Count > 0));
                _CanExecute = executable;
            }
            if (valid)
            {
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Ready"), row, col, _CategorizedCode?.Code.Title));
            }
            else
            {
                (int y, int x) = code.FindCoordinates(error);
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Warning"), row, col, y, x, _CategorizedCode?.Code.Title));
            }
        }

        /// <summary>
        /// Raised whenever the user requests to add a new character with the virtual keyboard
        /// </summary>
        public event EventHandler<char> CharInsertionRequested; 

        /// <summary>
        /// Raised whenever the current text is cleared
        /// </summary>
        public event EventHandler TextCleared; 

        /// <summary>
        /// Clears the current content in the document
        /// </summary>
        private void TryClearScreen()
        {
            Document.SetText(TextSetOptions.None, String.Empty);
            _CategorizedCode = null;
            Messenger.Default.Send(new SaveButtonsEnabledStatusChangedMessage(false, true));
            SendMessages();
            TextCleared?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Manages and executes an undo/redo request
        /// </summary>
        /// <param name="request">The requested operation</param>
        private void ManageUndoRedoRequest(UndoRedoOperation request)
        {
            // Execute the requested operation, if possible
            switch (request)
            {
                case UndoRedoOperation.Undo:
                    if (_CanUndo) Document.Undo();
                    break;
                case UndoRedoOperation.Redo:
                    if (_CanRedo) Document.Redo();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(request), request, null);
            }

            // Update the status
            UpdateCanUndoRedoStatus();
        }

        // Indicates whether or not the document can undo the latest changes
        private bool _CanUndo;

        // Indicates whether or not the document can redo the latest undone changes
        private bool _CanRedo;

        /// <summary>
        /// Updates the status of the undo and redo functions
        /// </summary>
        public void UpdateCanUndoRedoStatus()
        {
            // Get the updated status for the two features
            bool
                undo = Document.CanUndo(),
                redo = Document.CanRedo();

            // Send the messages if needed and update the local fields
            if (_CanUndo != undo)
            {
                _CanUndo = undo;
                Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.Undo, undo));
            }
            if (_CanRedo != redo)
            {
                _CanRedo = redo;
                Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.Redo, redo));
            }
        }

        #region Indentation info

        /// <summary>
        /// Updates the indentation info for a given state
        /// </summary>
        /// <param name="brackets">The collection of brackets and their position in the current text</param>
        public void UpdateIndentationInfo([CanBeNull] IReadOnlyList<(int, int, char)> brackets)
        {
            // // Check the info is available
            if (brackets == null || brackets.Count == 0)
            {
                Source.Clear();
                return;
            }
            int max = brackets.Max(entry => entry.Item1);

            // Updates the indentation info displayed on the IDE
            List<IDEIndentationLineInfo> source = new List<IDEIndentationLineInfo>();
            uint depth = 0;
            for (int i = 1; i <= max; i++)
            {
                IReadOnlyList<(int, int, char)> line = brackets.Where(info => info.Item1 == i).ToArray();
                if (line.Count == 0)
                {
                    source.Add(new IDEIndentationLineInfo(depth == 0 ? IDEIndentationInfoLineType.Empty : IDEIndentationInfoLineType.Straight));
                }
                else if (line[0].Item3 == '[')
                {
                    depth++;
                    source.Add(new IDEIndentationOpenBracketLineInfo(depth));
                }
                else if (line[0].Item3 == ']')
                {
                    depth--;
                    source.Add(new IDEIndentationLineInfo(IDEIndentationInfoLineType.ClosedBracket));
                }
            }
            
            // Update the source collection
            for (int i = 0; i < source.Count; i++)
            {
                // The source doesn't contain enough items
                if (Source.Count - 1 < i)
                {
                    Source.Add(source[i]);
                }

                // Replace the current item if needed
                IDEIndentationLineInfo
                    previous = Source[i],
                    next = source[i];

                if (previous is IDEIndentationOpenBracketLineInfo info &&
                    next is IDEIndentationOpenBracketLineInfo updated
                    ? info.Depth == updated.Depth
                    : previous.LineType == next.LineType)
                {
                    continue;
                }
                Source[i] = source[i];
            }

            // Remove the exceeding items
            int diff = Source.Count - source.Count;
            while (diff > 0)
            {
                Source.RemoveAt(Source.Count - 1);
                diff--;
            }
        }

        #endregion

        #region Git diff indicators

        /// <summary>
        /// Gets the items collection for the current instance
        /// </summary>
        [NotNull]
        public ObservableCollection<GitDiffLineStatus> DiffStatusSource { get; } = new ObservableCollection<GitDiffLineStatus>();

        // Updates the git diff indicators when the current source code is saved
        private void UpdateGitDiffStatusOnSave()
        {
            for (int i = 0; i < DiffStatusSource.Count; i++)
                if (DiffStatusSource[i] == GitDiffLineStatus.Edited)
                    DiffStatusSource[i] = GitDiffLineStatus.Saved;
        }

        /// <summary>
        /// Updates the diff indicators for the current source code being edited
        /// </summary>
        /// <param name="previous">The previous code</param>
        /// <param name="current">The current code</param>
        public void UpdateGitDiffStatus([NotNull] String previous, [NotNull] String current)
        {
            // Clear the current indicators if the two strings are the same
            if (previous.Equals(current))
            {
                DiffStatusSource.Clear();
                return;
            }

            String[]
                currentLines = current.Split('\r'),
                previousLines = previous.Replace("\n", "").Split('\r').Take(currentLines.Length).ToArray();
            List<GitDiffLineStatus> source = new List<GitDiffLineStatus>();
            for (int i = 0; i < currentLines.Length - 1; i++)
            {
                if (i > previousLines.Length - 1) source.Add(GitDiffLineStatus.Edited);
                else source.Add(currentLines[i].Equals(previousLines[i]) ? GitDiffLineStatus.Undefined : GitDiffLineStatus.Edited);
                // TODO: actually implement this
            }

            // Update the source collection
            for (int i = 0; i < source.Count; i++)
            {
                // The source doesn't contain enough items
                if (DiffStatusSource.Count - 1 < i)
                {
                    DiffStatusSource.Add(source[i]);
                }

                // Replace the current item if needed
                if (source[i] != DiffStatusSource[i] &&
                    !(DiffStatusSource[i] == GitDiffLineStatus.Saved && source[i] == GitDiffLineStatus.Undefined))
                {
                    DiffStatusSource[i] = source[i];
                }
            }

            // Remove the exceeding items
            int diff = DiffStatusSource.Count - source.Count;
            while (diff > 0)
            {
                DiffStatusSource.RemoveAt(DiffStatusSource.Count - 1);
                diff--;
            }
        }

        #endregion
    }
}
