using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Text;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.EventArgs;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.DataModels.Misc.IDEIndentationGuides;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
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
        #region Local fields

        // The current document that's linked to the view
        private readonly ITextDocument Document;

        // A UI-bound function that asks the user to pick a name to save a new source code
        private readonly Func<String, Task<String>> SaveNameSelector;

        // A function that retrieves the list of breakpoints currently present in the code
        private readonly Func<IReadOnlyCollection<int>> BreakpointsExtractor;

        #endregion

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
            Messenger.Default.Register<IDEAutosaveTriggeredMessage>(this, async m =>
            {
                await TryAutosaveAsync();
                m.ReportAutosaveCompleted();
            });
        }

        // Indicates whether or not the view model instance hasn't already been enabled before
        private bool _Startup = true;

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
                        Messenger.Default.Register<PlayScriptMessage>(this, m => PlayRequested?.Invoke(this, new PlayRequestedEventArgs(m.StdinBuffer, m.Mode, m.Type == ScriptPlayType.Debug)));
                        Messenger.Default.Register<SaveSourceCodeRequestMessage>(this, m => ManageSaveCodeRequest(m.RequestType).Forget());
                        Messenger.Default.Register<IDEUndoRedoRequestMessage>(this, m => ManageUndoRedoRequest(m.Operation));
                        Messenger.Default.Register<IDENewLineRequestedMessage>(this, m => NewLineInsertionRequested?.Invoke(this, EventArgs.Empty));
                        Messenger.Default.Register<VirtualArrowKeyPressedMessage>(this, m => ManageVirtualArrowKeyPressed(m.Direction));
                        Messenger.Default.Register<SourceCodeLoadingRequestedMessage>(this, m =>
                        {
                            _CategorizedCode = m.RequestedCode;
                            LoadedCodeChanged?.Invoke(this, m.RequestedCode.Code);
                            Messenger.Default.Send(new SaveButtonsEnabledStatusChangedMessage(m.RequestedCode.Type != SavedSourceCodeType.Sample, true));
                            Messenger.Default.Send(new IDEPendingChangesStatusChangedMessage(false));
                        });
                        Messenger.Default.Register<IDEDeleteCharacterRequestMessage>(this, m =>
                        {
                            if (Document.Selection.Length == 0 && Document.Selection.StartPosition > 0) Document.Selection.StartPosition--;
                            Document.Selection.SetText(TextSetOptions.None, String.Empty);
                        });
                        SendMessages();
                        if (_Startup)
                        {
                            Messenger.Default.Send(new SaveButtonsEnabledStatusChangedMessage(false, true)); // Default save buttons status
                            _Startup = false;
                        }
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

        #region Events

        /// <summary>
        /// Raised whenever the current loaded source code changes
        /// </summary>
        public event EventHandler<SourceCode> LoadedCodeChanged;

        /// <summary>
        /// Raised whenever the user requests to play the current script
        /// </summary>
        public event EventHandler<PlayRequestedEventArgs> PlayRequested;

        /// <summary>
        /// Raised whenever the user requests to add a new character with the virtual keyboard
        /// </summary>
        public event EventHandler<char> CharInsertionRequested;

        /// <summary>
        /// Raised whenever the user requests to insert a new '\r' character at the current position
        /// </summary>
        public event EventHandler NewLineInsertionRequested;

        /// <summary>
        /// Raised whenever the current text is cleared
        /// </summary>
        public event EventHandler TextCleared;

        #endregion

        // Manages the selection when a virtual arrow key is pressed
        private void ManageVirtualArrowKeyPressed(VirtualArrow direction)
        {
            switch (direction)
            {
                case VirtualArrow.Up:
                    Document.Selection.MoveUp(TextRangeUnit.Line, 1, false);
                    break;
                case VirtualArrow.Left:
                    Document.Selection.MoveLeft(TextRangeUnit.Character, 1, false);
                    break;
                case VirtualArrow.Down:
                    Document.Selection.MoveDown(TextRangeUnit.Line, 1, false);
                    break;
                case VirtualArrow.Right:
                    Document.Selection.MoveRight(TextRangeUnit.Character, 1, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        /// <summary>
        /// Autosaves the current code file, if there's a loaded document in the IDE
        /// </summary>
        private async Task TryAutosaveAsync()
        {
            // State check
            if (LoadedCode == null) return;

            // Get the text and the breakpoints
            Document.GetText(TextGetOptions.None, out String text);
            IReadOnlyCollection<int>
                raw = BreakpointsExtractor(),
                breakpoints = raw.Count > 0 ? raw : null;
            if (text[text.Length - 1] == '\r') text = text.Substring(0, text.Length - 1); // Remove final '\r' to avoid addednew lines

            // Save the file as requested
            await SQLiteManager.Instance.SaveCodeAsync(LoadedCode, text, breakpoints);
            UpdateGitDiffStatusOnSave();
            Messenger.Default.Send(new IDEPendingChangesStatusChangedMessage(false));
        }

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
                            NotificationsManager.Instance.ShowNotification(0xEC24.ToSegoeMDL2Icon(), LocalizationManager.GetResource("CodeSaved"),
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
            Messenger.Default.Send(new IDEPendingChangesStatusChangedMessage(false));
        }

        // Indicates whether or not the IDE contains at least a valid operator
        private bool _CanExecute;

        /// <summary>
        /// Sends the status info messages for the current state
        /// </summary>
        public void SendMessages([CanBeNull] String code = null)
        {
            // Initial checks
            if (code == null) Document.GetText(TextGetOptions.None, out code);
            Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.ClearScreen, code.Length > 1));
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(code);
            Coordinate previous = code.FindCoordinates(Document.Selection.StartPosition);
            bool executable = Brainf_ckInterpreter.FindOperators(code) && result.Valid;
            Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.Delete, code.Length > 1 && Document.Selection.StartPosition > 0));

            // Executable code
            if (_CanExecute != executable)
            {
                Messenger.Default.Send(new IDEExecutableStatusChangedMessage(executable));
                _CanExecute = executable;
            }

            // Syntax status
            if (result.Valid)
            {
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Ready"), previous.Y, previous.X, _CategorizedCode?.Code.Title));
            }
            else
            {
                Coordinate coordinate = code.FindCoordinates(result.ErrorPosition);
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Warning"),
                    previous.Y, previous.X, coordinate.Y, coordinate.X, _CategorizedCode?.Code.Title));
            }
        }

        /// <summary>
        /// Sends a message to disable the debug mode when the current breakpoints get deleted
        /// </summary>
        public void SignalBreakpointsDeleted() => Messenger.Default.Send(new DebugStatusChangedMessage(false));

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
        public async Task UpdateIndentationInfo([CanBeNull] IReadOnlyList<IndentationCoordinateEntry> brackets)
        {
            // // Check the info is available
            if (brackets == null || brackets.Count == 0)
            {
                Source.Clear();
                return;
            }

            // Prepare the updated source collection
            List<IDEIndentationLineInfo> source = await Task.Run(() =>
            {
                // Get the max reached line number
                int max = brackets.Max(entry => entry.Position.Y);

                // Updates the indentation info displayed on the IDE
                List<IDEIndentationLineInfo> temp = new List<IDEIndentationLineInfo>();
                uint depth = 0;
                for (int i = 1; i <= max; i++)
                {
                    // Parse the first item
                    IReadOnlyList<IndentationCoordinateEntry> entries = brackets.Where(info => info.Position.Y == i).ToArray();
                    if (entries.Count == 0)
                    {
                        temp.Add(new IDEIndentationLineInfo(depth == 0 ? IDEIndentationInfoLineType.Empty : IDEIndentationInfoLineType.Straight));
                    }
                    else if (entries.Count > 1 && entries.Sum(entry => entry.Bracket == '[' ? 1 : -1) == 0)
                    {
                        // Edge case: multiple brackets opened and closed on the same line
                        temp.Add(new IDEIndentationOpenBracketLineInfo(depth + 1, true));
                        continue;
                    }
                    else if (entries[0].Bracket == '[')
                    {
                        depth++;
                        temp.Add(new IDEIndentationOpenBracketLineInfo(depth, false));
                    }
                    else if (entries[0].Bracket == ']')
                    {
                        depth--;
                        temp.Add(new IDEIndentationLineInfo(IDEIndentationInfoLineType.ClosedBracket));
                    }

                    // Edge case, multiple brackets on the same line
                    if (entries.Count > 1)
                    {
                        foreach (IndentationCoordinateEntry entry in entries.Skip(1))
                        {
                            if (entry.Bracket == '[') depth++;
                            else if (entry.Bracket == ']') depth--;
                        }
                    }
                }
                return temp;
            });
            
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
        public async Task UpdateGitDiffStatus([NotNull] String previous, [NotNull] String current)
        {
            // Clear the current indicators if the two strings are the same
            if (previous.Equals(current))
            {
                DiffStatusSource.Clear();
                Messenger.Default.Send(new IDEPendingChangesStatusChangedMessage(false));
                return;
            }

            // Prepare the updated source
            List<GitDiffLineStatus> source = await Task.Run(() =>
            {
                String[]
                    currentLines = current.Split('\r'),
                    previousLines = previous.Replace("\n", "").Split('\r').Take(currentLines.Length).ToArray();
                List<GitDiffLineStatus> temp = new List<GitDiffLineStatus>();
                for (int i = 0; i < currentLines.Length - 1; i++)
                {
                    if (i > previousLines.Length - 1) temp.Add(GitDiffLineStatus.Edited);
                    else temp.Add(currentLines[i].Equals(previousLines[i]) ? GitDiffLineStatus.Undefined : GitDiffLineStatus.Edited);
                    // TODO: actually implement this
                }
                return temp;
            });

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
            Messenger.Default.Send(new IDEPendingChangesStatusChangedMessage(source.Any(state => state == GitDiffLineStatus.Edited)));
        }

        #endregion
    }
}
