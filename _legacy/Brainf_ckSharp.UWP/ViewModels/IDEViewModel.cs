using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Text;
using Brainf_ck_sharp.Legacy.UWP.DataModels;
using Brainf_ck_sharp.Legacy.UWP.DataModels.EventArgs;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.CharactersInfo;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.IDEIndentationGuides;
using Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite;
using Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp.Legacy.UWP.Enums;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Settings;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using Brainf_ck_sharp.Legacy.UWP.Messages.Actions;
using Brainf_ck_sharp.Legacy.UWP.Messages.IDE;
using Brainf_ck_sharp.Legacy.UWP.Messages.Requests;
using Brainf_ck_sharp.Legacy.UWP.Messages.UI;
using Brainf_ck_sharp.Legacy.UWP.PopupService;
using Brainf_ck_sharp.Legacy.UWP.PopupService.Misc;
using Brainf_ck_sharp.Legacy.UWP.SQLiteDatabase;
using Brainf_ck_sharp.Legacy.UWP.ViewModels.Abstract;
using Brainf_ckSharp.Legacy;
using Brainf_ckSharp.Legacy.ReturnTypes;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.ViewModels
{
    public class IDEViewModel : ItemsCollectionViewModelBase<IDEIndentationLineInfo>
    {
        #region Local fields

        // The current document that's linked to the view
        private readonly ITextDocument Document;

        // A UI-bound function that asks the user to pick a name to save a new source code
        private readonly Func<string, Task<string>> SaveNameSelector;

        // A function that retrieves the list of breakpoints currently present in the code
        private readonly Func<IReadOnlyCollection<int>> BreakpointsExtractor;

        #endregion

        /// <summary>
        /// Creates a new instance to manage the IDE
        /// </summary>
        /// <param name="document">The target document that contains the source code to edit</param>
        /// <param name="nameSelector">A function that prompts the user to enter a name to save a new source code in the app</param>
        /// <param name="breakpointsExtractor">A function that retrieves the active breakpoints</param>
        public IDEViewModel([NotNull] ITextDocument document, [NotNull] Func<string, 
            Task<string>> nameSelector, [NotNull] Func<IReadOnlyCollection<int>> breakpointsExtractor)
        {
            Document = document;
            SaveNameSelector = nameSelector;
            BreakpointsExtractor = breakpointsExtractor;
            Messenger.Default.Register<IDEAutosaveTriggeredMessage>(this, async m =>
            {
                await TryAutosaveAsync();
                m.ReportResult(Unit.Instance);
            });
            Messenger.Default.Register<IDEUnsavedChangesRequestMessage>(this, m =>
            {
                Document.GetText(TextGetOptions.None, out string code);
                code = code.Substring(0, code.Length - 1); // Remove trailing \r
                if (CategorizedCode == null) m.ReportResult(!string.IsNullOrEmpty(code));
                else m.ReportResult(!CategorizedCode.Code.Code.Equals(code));
            });
        }

        // Indicates whether or not the view model instance hasn't already been enabled before
        private bool _Startup = true;

        #region Public parameters

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
                        Messenger.Default.Register<OperatorAddedMessage>(this, op => CharInsertionRequested?.Invoke(this, op.Value));
                        Messenger.Default.Register<ClearScreenMessage>(this, m => TryClearScreen());
                        Messenger.Default.Register<PlayScriptMessage>(this, m => PlayRequested?.Invoke(this, new PlayRequestedEventArgs(m.StdinBuffer, m.Mode, m.Type == ScriptPlayType.Debug)));
                        Messenger.Default.Register<SaveSourceCodeRequestMessage>(this, m => ManageSaveCodeRequest(m.Value).Forget());
                        Messenger.Default.Register<IDEUndoRedoRequestMessage>(this, m => ManageUndoRedoRequest(m.Value));
                        Messenger.Default.Register<IDENewLineRequestedMessage>(this, m => NewLineInsertionRequested?.Invoke(this, EventArgs.Empty));
                        Messenger.Default.Register<VirtualArrowKeyPressedMessage>(this, m => ManageVirtualArrowKeyPressed(m.Value));
                        Messenger.Default.Register<SourceCodeLoadingRequestedMessage>(this, m =>
                        {
                            // Skip reloading the current code
                            if (m.Source == SavedCodeLoadingSource.Timeline && CategorizedCode?.Code.Uid.Equals(m.RequestedCode.Code.Uid) == true)
                            {
                                Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
                                NotificationsManager.Instance.ShowNotification(0xE148.ToSegoeMDL2Icon(), LocalizationManager.GetResource("AlreadyLoadedTitle"), LocalizationManager.GetResource("AlreadyLoadedBody"), NotificationType.Default);
                                return;
                            }

                            // Load the requested code
                            CategorizedCode = m.RequestedCode;
                            LoadedCodeChanged?.Invoke(this, (InitialWorkSessionCode, m.RequestedCode.Code.Breakpoints));
                            Messenger.Default.Send(new SaveButtonsEnabledStatusChangedMessage(m.RequestedCode.Type != SavedSourceCodeType.Sample, true));
                            Messenger.Default.Send(new IDEPendingChangesStatusChangedMessage(false));
                        });
                        Messenger.Default.Register<IDEDeleteCharacterRequestMessage>(this, m =>
                        {
                            if (Document.Selection.Length == 0 && Document.Selection.StartPosition > 0) Document.Selection.StartPosition--;
                            Document.Selection.SetText(TextSetOptions.None, string.Empty);
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
        /// Gets or sets the code the user is currently working on
        /// </summary>
        [CanBeNull]
        public CategorizedSourceCode CategorizedCode
        {
            get => _CategorizedCode;
            set
            {
                if (_CategorizedCode != value)
                {
                    if (value != null) Messenger.Default.Send(new WorkingSourceCodeChangedMessage(value));
                    _CategorizedCode = value;
                    if (value == null) InitialWorkSessionCode = string.Empty;
                    else if (value.Type == SavedSourceCodeType.Sample)
                    {
                        InitialWorkSessionCode = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.BracketsStyle)) == 1
                            ? Regex.Replace(value.Code.Code, @"(?<=[\+-><\[\]\(\):\.,])\r\n\t*?\[\r\n", "[\r\n")
                            : value.Code.Code;
                    }
                    else InitialWorkSessionCode = value.Code.Code;
                }
            }
        }

        /// <summary>
        /// Gets the source code that was loaded at the beginning of the current editing session
        /// </summary>
        [NotNull]
        public string InitialWorkSessionCode { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the source code currently loaded, if present
        /// </summary>
        [CanBeNull]
        public SourceCode LoadedCode => CategorizedCode?.Code;

        #endregion

        #region Events

        /// <summary>
        /// Raised whenever the current loaded source code changes
        /// </summary>
        public event EventHandler<(string Code, byte[] Breakpoints)> LoadedCodeChanged;

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
            Document.GetText(TextGetOptions.None, out string text);
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
            Document.GetText(TextGetOptions.None, out string text);
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
                    InitialWorkSessionCode = text;
                    UpdateGitDiffStatusOnSave();
                    break;

                // Save the current code as a new file
                case CodeSaveType.SaveAs:
                    string name = await SaveNameSelector(text);
                    if (!string.IsNullOrEmpty(name))
                    {
                        AsyncOperationResult<CategorizedSourceCode> result = await SQLiteManager.Instance.SaveCodeAsync(name, text, breakpoints);
                        if (result)
                        {
                            // Update the local code reference, the git diff indicators and notify the UI with the new save buttons state
                            CategorizedCode = result.Result;
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
        public void SendMessages([CanBeNull] string code = null)
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
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Ready"), previous.Y, previous.X, CategorizedCode?.Code.Title));
            }
            else
            {
                Coordinate coordinate = code.FindCoordinates(result.ErrorPosition);
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Warning"),
                    previous.Y, previous.X, coordinate.Y, coordinate.X, CategorizedCode?.Code.Title));
            }
        }

        /// <summary>
        /// Sends a message to disable the debug mode when the current breakpoints get deleted
        /// </summary>
        public void SignalBreakpointsDeleted() => Messenger.Default.Send(new DebugStatusChangedMessage(false));

        /// <summary>
        /// Clears the current content in the document
        /// </summary>
        private async void TryClearScreen()
        {
            // Ask for confirmation
            if (AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ProtectUnsavedChanges)) &&
                await Messenger.Default.RequestAsync<bool, IDEUnsavedChangesRequestMessage>() &&
                await FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("UnsavedChangesTitle"),
                    LocalizationManager.GetResource("UnsavedChangesClear"), LocalizationManager.GetResource("Ok")) == FlyoutResult.Canceled)
            {
                return;
            }

            // Clear the screen
            Document.SetText(TextSetOptions.None, string.Empty);
            CategorizedCode = null;
            Messenger.Default.Send(new SaveButtonsEnabledStatusChangedMessage(false, true));
            SendMessages();
            TextCleared?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets whether or not the current text change is caused by an undo/redo action
        /// </summary>
        public bool DisableUndoGroupManagement { get; private set; }

        /// <summary>
        /// Manages and executes an undo/redo request
        /// </summary>
        /// <param name="request">The requested operation</param>
        private void ManageUndoRedoRequest(UndoRedoOperation request)
        {
            // Execute the requested operation, if possible
            DisableUndoGroupManagement = true;
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
            DisableUndoGroupManagement = false;
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

        // The local backup of the brackets info
        [CanBeNull]
        private IReadOnlyList<CharacterWithCoordinates> _Brackets;

        // Synchronization semaphore for the brackets field
        private readonly SemaphoreSlim BracketsSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Updates the indentation info for a given state
        /// </summary>
        /// <param name="brackets">The collection of brackets and their position in the current text</param>
        [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
        public async Task UpdateIndentationInfo([CanBeNull] IReadOnlyList<CharacterWithCoordinates> brackets)
        {
            // Lock
            await BracketsSemaphore.WaitAsync();

            // Check the info is available
            if (brackets == null || brackets.Count == 0)
            {
                Source.Clear();
                _Brackets = null;
                BracketsSemaphore.Release();
                return;
            }

            // Prepare the updated source collection
            List<IDEIndentationLineInfo> source = await Task.Run(() =>
            {
                // Repetition check
                if (_Brackets != null && _Brackets.Count == brackets.Count)
                {
                    bool equals = true;
                    for (int i = 0; i < brackets.Count; i++)
                    {
                        CharacterWithCoordinates
                            backup = _Brackets[i],
                            local = brackets[i];
                        if (backup.Character != local.Character ||
                            backup.Position.Y != local.Position.Y)
                        {
                            equals = false;
                            break;
                        }
                    }
                    if (equals) return null;
                    _Brackets = brackets;
                }
                else _Brackets = brackets;

                // Get the max reached line number
                int max = brackets.Max(entry => entry.Position.Y);

                // Updates the indentation info displayed on the IDE
                List<IDEIndentationLineInfo> temp = new List<IDEIndentationLineInfo>();
                uint depth = 0, nested = 0;
                bool function = false;
                for (int i = 1; i <= max; i++)
                {
                    // Parse the first item
                    IReadOnlyList<CharacterWithCoordinates> entries = brackets.Where(info => info.Position.Y == i).ToArray();
                    if (entries.Count == 0)
                    {
                        // No brackets on the current line: keep the current state
                        temp.Add(new IDEIndentationLineInfo(depth == 0 && nested == 0 && !function ? IDEIndentationInfoLineType.Empty : IDEIndentationInfoLineType.Straight));
                    }
                    else if (entries.Count == 1)
                    {
                        // Display the single bracket on the line
                        switch (entries[0].Character)
                        {
                            case '[':
                                temp.Add(new IDEIndentationOpenLoopBracketLineInfo(function ? ++nested : ++depth, false,
                                    function ? IDEIndentationInfoOpenLoopBracketType.InFunction : IDEIndentationInfoOpenLoopBracketType.Default));
                                break;
                            case ']':
                                if (function) nested--;
                                else depth--;
                                temp.Add(new IDEIndentationLineInfo(IDEIndentationInfoLineType.ClosedBracket));
                                break;
                            case '(':
                                function = true;
                                temp.Add(new IDEIndentationFunctionBracketInfo(depth > 0, IDEIndentationInfoLineType.OpenFunctionBracket));
                                break;
                            case ')':
                                function = false;
                                temp.Add(new IDEIndentationLineInfo(IDEIndentationInfoLineType.ClosedBracket));
                                break;
                            default:
                                throw new InvalidOperationException("Invalid bracket character");
                        }
                    }
                    else if (entries.Count == 2 && entries[0].Character == '(' && entries[1].Character == ')')
                    {
                        // Standalone function on a single line
                        temp.Add(new IDEIndentationFunctionBracketInfo(depth > 0, IDEIndentationInfoLineType.SelfContainedFunction));
                    }
                    else if (entries.Count == 2 && entries[0].Character == '[' && entries[1].Character == ']')
                    {
                        // Standalone function on a single line
                        temp.Add(new IDEIndentationOpenLoopBracketLineInfo(depth + 1, true,
                            function ? IDEIndentationInfoOpenLoopBracketType.InFunction : IDEIndentationInfoOpenLoopBracketType.Default));
                    }
                    else if (!entries.Any(e => e.Character == '(' || e.Character == ')'))
                    {
                        // Function to calculate the updated depth
                        uint CalculateAndShowDepth(uint target)
                        {
                            int sum = entries.Sum(e => e.Character == '[' ? 1 : -1);
                            if ((int)target + sum < 0) throw new InvalidOperationException("Invalid brackets sequence");
                            uint final = (uint)((int)target + sum);
                            if (target == 0 && final == 0)
                            {
                                // Brackets opened and closed on the same line
                                IDEIndentationInfoOpenLoopBracketType type;
                                if (function) type = IDEIndentationInfoOpenLoopBracketType.InFunction;
                                else if (depth > 0) type = IDEIndentationInfoOpenLoopBracketType.Nested;
                                else type = IDEIndentationInfoOpenLoopBracketType.Default;
                                temp.Add(new IDEIndentationOpenLoopBracketLineInfo(target + 1, true, type));
                                return final;
                            }
                            if (final == 0) temp.Add(new IDEIndentationLineInfo(IDEIndentationInfoLineType.ClosedBracket)); // All brackets closed
                            else if (final >= target)
                            {
                                IDEIndentationInfoOpenLoopBracketType type;
                                if (function) type = IDEIndentationInfoOpenLoopBracketType.InFunction;
                                else if (target > 0) type = IDEIndentationInfoOpenLoopBracketType.Nested;
                                else type = IDEIndentationInfoOpenLoopBracketType.Default;
                                temp.Add(new IDEIndentationOpenLoopBracketLineInfo(final, false, type)); // New indentation level
                            }
                            else temp.Add(new IDEIndentationLineInfo(IDEIndentationInfoLineType.ClosedBracket)); // Indentation depth decreased
                            return final;
                        }

                        // Display the right depth level
                        if (function) nested = CalculateAndShowDepth(nested);
                        else depth = CalculateAndShowDepth(depth);
                    }
                    else
                    {
                        // Calculate the final state at the end of the line
                        bool call = function, open = false, definition = false;
                        uint backup = depth;
                        foreach (CharacterWithCoordinates info in entries)
                        {
                            switch (info.Character)
                            {
                                case '[':
                                    if (function) nested++;
                                    else depth++;
                                    break;
                                case ']':
                                    if (function) nested--;
                                    else depth--;
                                    break;
                                case '(':
                                    function = true;
                                    open = true;
                                    break;
                                case ')':
                                    function = false;
                                    if (open) definition = true;
                                    break;
                                default:
                                    throw new InvalidOperationException("Invalid bracket character");
                            }
                        }

                        // Display the right indicator
                        if (call && !function)
                        {
                            // Previous call to function, now closed
                            if (definition)
                            {
                                // A new function has the precedence
                                temp.Add(new IDEIndentationFunctionBracketInfo(depth > 0, IDEIndentationInfoLineType.SelfContainedFunction));
                            }
                            else if (depth > 0 && backup != depth)
                            {
                                // New depth level
                                temp.Add(new IDEIndentationOpenLoopBracketLineInfo(depth, false, IDEIndentationInfoOpenLoopBracketType.Nested));
                            }
                            else temp.Add(new IDEIndentationLineInfo(IDEIndentationInfoLineType.ClosedBracket)); // No indentation
                        }
                        else if (!call && function)
                        {
                            // Not in a call, but new function invoked on this line
                            temp.Add(definition 
                                ? new IDEIndentationFunctionBracketInfo(depth > 0, IDEIndentationInfoLineType.SelfContainedFunction)  // Self-contained precedence
                                : new IDEIndentationFunctionBracketInfo(depth > 0, IDEIndentationInfoLineType.OpenFunctionBracket));  // New function call otherwise
                        }
                        else if (definition && !function)
                        {
                            // Precedence for the standalone function
                            temp.Add(new IDEIndentationFunctionBracketInfo(depth > 0, IDEIndentationInfoLineType.SelfContainedFunction));
                        }
                        else if (function) temp.Add(new IDEIndentationFunctionBracketInfo(depth > 0, IDEIndentationInfoLineType.OpenFunctionBracket));
                        else throw new InvalidOperationException("There must be either a ( or ) character at this point");
                    }
                }
                return temp;
            });

            // Update the UI if needed
            if (source != null)
            {
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

                    // Check the current swap
                    if (previous.Equals(next)) continue;
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

            // Release
            BracketsSemaphore.Release();
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
        /// <param name="current">The current code</param>
        public async Task UpdateGitDiffStatus([NotNull] string current)
        {
            // Clear the current indicators if the two strings are the same
            if (InitialWorkSessionCode.Equals(current))
            {
                DiffStatusSource.Clear();
                Messenger.Default.Send(new IDEPendingChangesStatusChangedMessage(false));
            }

            // Prepare the updated source
            GitDiffLineStatus[] source = await Task.Run(() =>
            {
                IInlineDiffBuilder builder = new InlineDiffBuilder(new Differ());
                string trimmed = current.Replace("\n", "");
                trimmed = trimmed.Substring(0, trimmed.Length - 1);
                return (
                        from line in builder.BuildDiffModel(InitialWorkSessionCode, trimmed).Lines
                        where line.Type != ChangeType.Deleted
                        select line.Type == ChangeType.Unchanged
                            ? GitDiffLineStatus.Undefined
                            : GitDiffLineStatus.Edited).ToArray();
            });

            // Update the source collection
            for (int i = 0; i < source.Length; i++)
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
            int diff = DiffStatusSource.Count - source.Length;
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
