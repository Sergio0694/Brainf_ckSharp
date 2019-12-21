using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Brainf_ck_sharp.Legacy.UWP.DataModels;
using Brainf_ck_sharp.Legacy.UWP.DataModels.EventArgs;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.CharactersInfo;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp.Legacy.UWP.Enums;
using Brainf_ck_sharp.Legacy.UWP.Helpers;
using Brainf_ck_sharp.Legacy.UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Settings;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using Brainf_ck_sharp.Legacy.UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp.Legacy.UWP.Messages.Actions;
using Brainf_ck_sharp.Legacy.UWP.Messages.IDE;
using Brainf_ck_sharp.Legacy.UWP.Messages.Settings;
using Brainf_ck_sharp.Legacy.UWP.Messages.UI;
using Brainf_ck_sharp.Legacy.UWP.PopupService;
using Brainf_ck_sharp.Legacy.UWP.PopupService.Misc;
using Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts;
using Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.SnippetsMenu;
using Brainf_ck_sharp.Legacy.UWP.ViewModels;
using Brainf_ckSharp.Legacy;
using Brainf_ckSharp.Legacy.ReturnTypes;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.Helpers;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp.Legacy.UWP.Views
{
    public sealed partial class IDEView : UserControl, ICodeWorkspacePage
    {
        public IDEView()
        {
            Loaded += IDEView_Loaded;
            this.InitializeComponent();
            LinesGrid.SetVisualOpacity(0);
            ApplyUITheme();
            ApplyCustomTabSpacing();
            DataContext = new IDEViewModel(EditBox.Document, PickSaveNameAsync, () => BreakpointsInfo.Keys);
            ViewModel.PlayRequested += ViewModel_PlayRequested;
            ViewModel.LoadedCodeChanged += ViewModel_LoadedCodeChanged;
            ViewModel.TextCleared += ViewModel_TextCleared;
            ViewModel.CharInsertionRequested += ViewModel_CharInsertionRequested;
            ViewModel.NewLineInsertionRequested += ViewModel_NewLineInsertionRequested;
            EditBox.Document.GetText(TextGetOptions.None, out string text);
            _PreviousText = text;
            _WhitespacesRenderingEnabled = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.RenderWhitespaces));
            Messenger.Default.Register<IDESettingsChangedMessage>(this, ApplyIDESettings);
            Messenger.Default.Register<CodeSnippetSelectedMessage>(this, m =>
            {
                LoadCode(m.Snippet.Code, false, m.Snippet.CursorOffset);
                if (m.Source == PointerDeviceType.Mouse) EditBox.Focus(FocusState.Programmatic);
            });
            Messenger.Default.Register<ClipboardOperationRequestMessage>(this, m => HandleClipboardOperationRequest(m.Value));
        }

        /// <inheritdoc cref="ICodeWorkspacePage"/>
        public string SourceCode
        {
            get
            {
                EditBox.Document.GetText(TextGetOptions.None, out string code);
                return code;
            }
        }

        #region IDE theme

        // Updates the general UI settings
        private void ApplyUITheme()
        {
            RootGrid.Background = new SolidColorBrush(Brainf_ckFormatterHelper.Instance.CurrentTheme.Background);
            BreakpointsCanvas.Background = new SolidColorBrush(Brainf_ckFormatterHelper.Instance.CurrentTheme.BreakpointsPaneBackground);
            LineBlock.Foreground = new SolidColorBrush(Brainf_ckFormatterHelper.Instance.CurrentTheme.LineNumberColor);
            if (Brainf_ckFormatterHelper.Instance.CurrentTheme.LineHighlightStyle == LineHighlightStyle.Outline)
            {
                CursorBorder.BorderThickness = new Thickness(2);
                CursorBorder.BorderBrush = Brainf_ckFormatterHelper.Instance.CurrentTheme.LineHighlightColor.ToBrush();
                CursorBorder.Background = Colors.Transparent.ToBrush();
                Canvas.SetZIndex(BracketsParentGrid, 0);
                Canvas.SetZIndex(CursorBorderParentCanvas, 1);
            }
            else
            {
                CursorBorder.BorderThickness = new Thickness(0);
                CursorBorder.Background = Brainf_ckFormatterHelper.Instance.CurrentTheme.LineHighlightColor.ToBrush();
                Canvas.SetZIndex(BracketsParentGrid, 1);
                Canvas.SetZIndex(CursorBorderParentCanvas, 0);
            }
        }

        // Applies the new IDE theme
        private void ApplyIDESettings(IDESettingsChangedMessage message)
        {
            // Disable the handlers
            EditBox.SelectionChanged -= EditBox_OnSelectionChanged;
            EditBox.TextChanged -= EditBox_OnTextChanged;

            // Update the tabs if needed
            if (message.TabsLengthChanged)
            {
                ApplyCustomTabSpacing();
                if (!message.WhitespacesChanged || !message.FontChanged && _WhitespacesRenderingEnabled)
                {
                    ClearControlCharacters();
                    RenderControlCharacters();
                }
                if (!message.FontChanged) UpdateCursorRectangleAndIndicatorUI();
            }

            // Update the render whitespaces setting
            if (message.WhitespacesChanged)
            {
                bool renderWhitespaces = _WhitespacesRenderingEnabled = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.RenderWhitespaces));
                if (renderWhitespaces && !message.FontChanged) RenderControlCharacters();
                else ClearControlCharacters();
            }

            // Update the font type if needed
            if (message.FontChanged)
            {
                // Refresh the text UI
                string name = AppSettingsManager.Instance.GetValue<string>(nameof(AppSettingsKeys.SelectedFontName));
                if (InstalledFont.TryGetFont(name, out InstalledFont font))
                {
                    LineBlock.FontFamily = font.Family;
                    EditBox.SetFontFamily(name);
                    AdjustOverlaysUIOnFontChanged(font);
                    UpdateCursorRectangleAndIndicatorUI(); // Adjust the cursor position (a different font can have a different height)
                    if (_WhitespacesRenderingEnabled)
                    {
                        ClearControlCharacters();
                        RenderControlCharacters();
                    }
                }
            }

            // Update the current UI theme
            if (message.ThemeChanged)
            {
                // Update the theme
                Brainf_ckFormatterHelper.Instance.ReloadTheme();

                // Main UI
                ApplyUITheme();

                // Code highlight
                EditBox.Document.GetText(TextGetOptions.None, out string text);
                int count = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    char test = text[count++];
                    if (test < 33 || test > 126 && test < 161) continue;
                    ITextRange range = EditBox.Document.GetRange(i, i + 1);
                    char c = range.Character;
                    range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar(c);
                }

                // Release the UI
                Task.Delay(500).ContinueWith(t =>
                {
                    Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            // Column guides and breakpoints
            DrawBracketGuides(null, true).Forget();
            if (message.FontChanged || message.TabsLengthChanged)
            {
                int[] breakpoints = BreakpointsInfo.Keys.Select(i => i - 1).ToArray();
                RestoreBreakpoints(breakpoints);
                BracketGuidesCanvas.Invalidate();
            }

            // Restore the handlers
            EditBox.SelectionChanged += EditBox_OnSelectionChanged;
            EditBox.TextChanged += EditBox_OnTextChanged;
        }

        // Updates the tab length setting
        private void ApplyCustomTabSpacing()
        {
            int
                setting = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.TabLength)),
                spaces = 4 + setting * 2; // Spacing options range from 4 to 12 at indexes [0..4]
            EditBox.SetTabLength(spaces);
        }

        #endregion

        #region ViewModel main events

        // Loads the requested code, when the user selects a saved file from his library
        private void ViewModel_LoadedCodeChanged(object sender, (string Code, byte[] Breakpoints) args)
        {
            // Load the code
            LoadCode(args.Code, true);
            if (args.Breakpoints == null)
            {
                ClearBreakpoints();
                BracketGuidesCanvas.Invalidate();
            }
            else RestoreBreakpoints(BitHelper.Expand(args.Breakpoints));
            Messenger.Default.Send(new DebugStatusChangedMessage(BreakpointsInfo.Keys.Count > 0));

            // Restore the UI
            Task.Delay(250).ContinueWith(t =>
            {
                Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Handles the UI when the user clears the contents of the IDE
        private void ViewModel_TextCleared(object sender, EventArgs e)
        {
            _PreviousText = "\r";
            EditBox.ResetTextAndUndoStack();
            ApplyCustomTabSpacing();
            ClearBreakpoints();
            BracketGuidesCanvas.Invalidate();
            Messenger.Default.Send(new DebugStatusChangedMessage(BreakpointsInfo.Keys.Count > 0));
        }

        // Inserts a new line at the current position
        private void ViewModel_NewLineInsertionRequested(object sender, EventArgs e)
        {
            EditBox.Document.BeginUndoGroup();
            EditBox.Document.Selection.SetText(TextSetOptions.None, "\r");
            EditBox.Document.Selection.SetRange(EditBox.Document.Selection.StartPosition + 1, EditBox.Document.Selection.StartPosition + 1);
        }

        // Inserts a new character picked from the custom virtual keyboard
        private void ViewModel_CharInsertionRequested(object sender, char e)
        {
            EditBox.Document.BeginUndoGroup();
            EditBox.Document.Selection.SetText(TextSetOptions.None, e.ToString());
            EditBox.Document.Selection.SetRange(EditBox.Document.Selection.StartPosition + 1, EditBox.Document.Selection.StartPosition + 1);
        }

        // The maximum execution time for a script
        private const int TimeThreshold = 2000;

        // Indicates whether or not there is a script already executed (thread-safe on the UI dispatcher)
        private bool _ExecutionInProgress;

        private async void ViewModel_PlayRequested(object sender, PlayRequestedEventArgs e)
        {
            // Get the text and initialize the session
            if (_ExecutionInProgress) return;
            _ExecutionInProgress = true;
            EditBox.Document.GetText(TextGetOptions.None, out string text);
            Func<InterpreterExecutionSession> factory;
            if (e.Debug)
            {
                // Get the lines with a breakpoint and deconstruct the source in executable chuncks
                IReadOnlyCollection<int>
                    lines = BreakpointsInfo.Keys.OrderBy(key => key).Select(i => i - 1).ToArray(), // i -1 to move from 1 to 0-based indexes
                    indexes = text.FindLineIndexes(lines);
                List<string> chuncks = new List<string>();
                int previous = 0;
                foreach (int breakpoint in indexes.Concat(new[] { text.Length - 1 }))
                {
                    chuncks.Add(text.Substring(previous, breakpoint - previous));
                    previous = breakpoint;
                }
                factory = () => Brainf_ckInterpreter.InitializeSession(chuncks, e.Stdin, e.Mode, AppSettingsParser.InterpreterMemorySize, TimeThreshold);
            }
            else factory = () => Brainf_ckInterpreter.InitializeSession(new[] { text }, e.Stdin, e.Mode, AppSettingsParser.InterpreterMemorySize, TimeThreshold);

            // Display the execution popup
            IDERunResultFlyout flyout = new IDERunResultFlyout();
            await FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource(e.Debug ? "Debug" : "RunTitle"), flyout, null, new Thickness(),
                openCallback: () => Task.Delay(100).ContinueWith(t => flyout.ViewModel.InitializeAsync(factory), TaskScheduler.FromCurrentSynchronizationContext()));
            _ExecutionInProgress = false;
        }

        #endregion

        /// <summary>
        /// Prompts the user to select a file name to save the current source code
        /// </summary>
        /// <param name="code">The source code that's being saved</param>
        private async Task<string> PickSaveNameAsync(string code)
        {
            SaveCodePromptFlyout flyout = new SaveCodePromptFlyout(code, null);
            FlyoutResult result = await FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("SaveCode"), 
                flyout, null, new Thickness(12, 12, 16, 12), FlyoutDisplayMode.ActualHeight);
            return result == FlyoutResult.Confirmed ? flyout.Title : null;
        }

        // Indicates whether or not the control has already been loaded
        private bool _Loaded;

        // Initializes the scroll events for the code
        private void IDEView_Loaded(object sender, RoutedEventArgs e)
        {
            // Skip repeated calls
            if (_Loaded) return;

            // Font setup
            string name = AppSettingsManager.Instance.GetValue<string>(nameof(AppSettingsKeys.SelectedFontName));
            if (InstalledFont.TryGetFont(name, out InstalledFont font))
            {
                LineBlock.FontFamily = font.Family;
                EditBox.SetFontFamily(name);
                AdjustOverlaysUIOnFontChanged(font);
            }

            // Start the cursor animation and subscribe the scroller event
            CursorAnimation.Begin();

            // Setup the expression animations
            SetupExpressionAnimations();
            LinesGrid.StartCompositionFadeAnimation(null, 1, 100, 200, EasingFunctionNames.CircleEaseOut);
            _Loaded = true;
        }

        // Starts the animation to keep the UI synced with the IDE text
        private void SetupExpressionAnimations()
        {
            LinesGrid.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y, (float)(_Top - 12));
            IndentationInfoList.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y, (float)(_Top + 10));
            GitDiffListView.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y, (float)(_Top + 10));
            BracketGuidesCanvas.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y);
            BracketGuidesCanvas.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.X);
            CursorRectangle.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y, (float)(_Top + 8));
            CursorRectangle.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.X);
            CursorBorder.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y, (float)(_Top + 8));
            WhitespacesCanvas.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y, (float)(_Top + 10));
            WhitespacesCanvas.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.X);
        }

        /// <summary>
        /// Refreshes the visual elements that rely on the current window size
        /// </summary>
        public void RefreshUI()
        {
            SetupExpressionAnimations();
            DrawBracketGuides(null, true).Forget();
            int[] breakpoints = BreakpointsInfo.Keys.Select(i => i - 1).ToArray();
            RestoreBreakpoints(breakpoints);
        }

        public IDEViewModel ViewModel => DataContext.To<IDEViewModel>();

        // The current top height of the page header
        private double _Top;

        /// <summary>
        /// Adjusts the top margin of the content in the list
        /// </summary>
        /// <param name="height">The desired height</param>
        public void AdjustTopMargin(double height)
        {
            _Top = height;
            LinesGrid.SetVisualOffset(TranslationAxis.Y, (float)(height - 12)); // Adjust the initial offset of the line numbers and indicators
            IndentationInfoList.SetVisualOffset(TranslationAxis.Y, (float)(height + 10));
            GitDiffListView.SetVisualOffset(TranslationAxis.Y, (float)(height + 10));
            CursorTransform.X = 4;
            BracketGuidesCanvas.SetVisualOffset(TranslationAxis.Y, 0);
            WhitespacesCanvas.SetVisualOffset(TranslationAxis.Y, (float)(height + 10));
            EditBox.Padding = new Thickness(4, _Top + 8, 20, 20);
            EditBox.ScrollBarMargin = new Thickness(0, _Top, 0, 0);
        }

        // Updates the line numbers displayed next to the code box
        private async void EditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            DrawLineNumbers();
            DrawBracketGuides(null, false).ContinueWith(t =>
            {
                if (t.Result.Status != AsyncOperationStatus.RunToCompletion) return;
                ViewModel.UpdateIndentationInfo(t.Result.Result).Forget();
            }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
            EditBox.Document.GetText(TextGetOptions.None, out string code);
            ViewModel.UpdateGitDiffStatus(code).Forget();
            ViewModel.UpdateCanUndoRedoStatus();
            await RemoveUnvalidatedBreakpointsAsync(code);
            RefreshBreakpointsUI(code);

            // Whitespaces rendering
            if (_WhitespacesRenderingEnabled)
            {
                if (code.Length > 1) RenderControlCharacters();
                else ClearControlCharacters();
            }
        }

        /// <summary>
        /// Updates the line numbers shown next to the code edit box
        /// </summary>
        private async void DrawLineNumbers()
        {
            // Draw the line numbers in the TextBlock next to the code
            EditBox.Document.GetText(TextGetOptions.None, out string text);
            LineBlock.Text = await Task.Run(() =>
            {
                int count = text.Split('\r').Length;
                StringBuilder builder = new StringBuilder();
                for (int i = 1; i < count; i++)
                {
                    builder.Append($"\n{i}");
                }
                return builder.ToString();
            });
        }

        #region Bracket guides

        // The approximate height of a code line with the current font in use
        private double _ApproximateLineHeight =20;

        // The adjustment vertical offset for the breakpoint line indicators
        private double _BreakpointsLineOffset = -2;

        // Adjusts the UI of some of the UI overlays when the selected font changes
        private void AdjustOverlaysUIOnFontChanged([NotNull] InstalledFont font)
        {
            switch (font.Name)
            {
                case "Calibri":
                    LinesGridTransform.Y = 2;
                    BracketGuidesCanvasTransform.Y = -3;
                    _BreakpointsLineOffset = 1;
                    IndentationInfoListTransform.Y = 0;
                    _ApproximateLineHeight = CursorBorder.Height = CursorRectangle.Height = 18;
                    WhitespacesTransform.Y = 0;
                    break;
                case "Cambria":
                    LinesGridTransform.Y = 2;
                    BracketGuidesCanvasTransform.Y = -3;
                    _BreakpointsLineOffset = 1;
                    IndentationInfoListTransform.Y = 0;
                    _ApproximateLineHeight = CursorBorder.Height = CursorRectangle.Height = 18;
                    WhitespacesTransform.Y = -1;
                    break;
                case "Consolas":
                    LinesGridTransform.Y = 2;
                    BracketGuidesCanvasTransform.Y = -4;
                    _BreakpointsLineOffset = 2;
                    IndentationInfoListTransform.Y = -1;
                    _ApproximateLineHeight = CursorBorder.Height = CursorRectangle.Height = 17;
                    WhitespacesTransform.Y = -2;
                    break;
                default:
                    LinesGridTransform.Y = 0;
                    BracketGuidesCanvasTransform.Y = 0;
                    IndentationInfoListTransform.Y = 0;
                    _ApproximateLineHeight = CursorBorder.Height = CursorRectangle.Height = 20;
                    WhitespacesTransform.Y = 0;
                    _BreakpointsLineOffset = -2;
                    break;
            }
            _ApproximateLineHeight = "Xg".MeasureText(15, font.Family).Height;
            RefreshListViewsStretch();
        }

        // The backup of the indexes of the brackets in the text
        private IReadOnlyList<CharacterWithCoordinates> _Brackets;

        // Brackets semaphore to avoid concurrent operations
        private readonly SemaphoreSlim BracketGuidesSemaphore = new SemaphoreSlim(1);

        // Cancellation token for concurrent operations started unvoluntarily
        private CancellationTokenSource _BracketGuidesCts;

        /// <summary>
        /// Redraws the column guides if necessary
        /// </summary>
        /// <param name="code">The current text, if already available</param>
        /// <param name="force">Indicates whether or not to always force a redraw of the column guides</param>
        private async Task<AsyncOperationResult<IReadOnlyList<CharacterWithCoordinates>>> DrawBracketGuides(string code, bool force)
        {
            // Get the text, clear the current guides and make sure the syntax is currently valid
            _BracketGuidesCts?.Cancel();
            await BracketGuidesSemaphore.WaitAsync();
            _BracketGuidesCts = new CancellationTokenSource();
            if (code == null) EditBox.Document.GetText(TextGetOptions.None, out code);

            // Check the current syntax
            SyntaxValidationResult result = await Task.Run(() => Brainf_ckInterpreter.CheckSourceSyntax(code));
            if (!result.Valid || _BracketGuidesCts.IsCancellationRequested)
            {
                _BracketsGuidesToRender = new List<LineCoordinates>();
                BracketGuidesCanvas.Invalidate();
                _Brackets = null;
                bool cancelled = _BracketGuidesCts.IsCancellationRequested;
                BracketGuidesSemaphore.Release();
                return cancelled
                    ? AsyncOperationStatus.Canceled
                    : AsyncOperationResult<IReadOnlyList<CharacterWithCoordinates>>.Explicit(null);
            }

            // Build the indexes for the current state
            Tuple<List<CharacterWithCoordinates>, bool> workingSet = await Task.Run(() =>
            {
                List<CharacterWithCoordinates> pairs = new List<CharacterWithCoordinates>();
                int index = 0;
                foreach (char c in code)
                {
                    if (c == '[' || c == ']' || c == '(' || c == ')')
                    {
                        Coordinate coordinate = code.FindCoordinates(index);
                        pairs.Add(new CharacterWithCoordinates(coordinate, c));
                    }
                    index++;
                }
                bool test = _Brackets?.Zip(pairs, (first, second) => first.Equals(second)).All(b => b) == true;
                return Tuple.Create(pairs, test);
            });

            // Check if the brackets haven't been changed or moved
            if (_Brackets != null &&
                _Brackets.Count == workingSet.Item1.Count &&
                workingSet.Item2 && !force ||
                _BracketGuidesCts.IsCancellationRequested)
            {
                BracketGuidesSemaphore.Release();
                return AsyncOperationResult<IReadOnlyList<CharacterWithCoordinates>>.Explicit(_Brackets);
            }
            _Brackets = workingSet.Item1;

            // Draw the guides for each brackets pair
            List<LineCoordinates> coordinates = new List<LineCoordinates>();
            int i = 0;
            foreach (char c in code)
            {
                // Get the index of the corresponding closing bracket (only if they're not on the same line)
                if (_BracketGuidesCts.IsCancellationRequested) break;
                if (c != '[')
                {
                    i++;
                    continue;
                }
                int height = 0, target = -1;
                bool newLine = false;
                for (int j = i + 1; j < code.Length; j++)
                {
                    char token = code[j];
                    if (token == '\r') newLine = true;
                    else if (token == '[') height++;
                    else if (token == ']')
                    {
                        if (height == 0)
                        {
                            if (newLine) target = j;
                            break;
                        }
                        height--;
                    }
                }
                if (target == -1)
                {
                    i++;
                    continue;
                }

                // Get the initial and ending range
                ITextRange range = EditBox.Document.GetRange(i, i);
                range.GetRect(PointOptions.Transform, out Rect open, out _);
                range = EditBox.Document.GetRange(target, target);
                range.GetRect(PointOptions.Transform, out Rect close, out _);

                // Render the new line guide
                coordinates.Add(new LineCoordinates((float)(close.Top - open.Bottom), (float)((close.X > open.X ? open.X : close.X) + 6), (float)(_Top + 30 + open.Top)));
                i++;
            }
            _BracketsGuidesToRender = coordinates;
            BracketGuidesCanvas.Invalidate();
            BracketGuidesSemaphore.Release();
            return workingSet.Item1;
        }

        // Gets the list of column guides that need to be rendered
        private IReadOnlyCollection<LineCoordinates> _BracketsGuidesToRender = new List<LineCoordinates>();

        // Draws the column guides in the Win2D canvas
        private void BracketGuidesCanvas_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            // Bracket guides
            IEnumerable<LineCoordinates> lines = _BracketsGuidesToRender;
            Color stroke = Brainf_ckFormatterHelper.Instance.CurrentTheme.BracketsGuideColor;
            if (Brainf_ckFormatterHelper.Instance.CurrentTheme.BracketsGuideStrokesLength.HasValue)
            {
                // Dashed line
                int dash = Brainf_ckFormatterHelper.Instance.CurrentTheme.BracketsGuideStrokesLength.Value;
                CanvasStrokeStyle style = new CanvasStrokeStyle { CustomDashStyle = new[] { dash - 1f, dash + 1f } };
                foreach (LineCoordinates line in lines)
                {
                    args.DrawingSession.DrawLine(line.X + 0.5f, line.Y - 0.5f, line.X + 0.5f, line.Y + line.Height + 0.5f, stroke, 1, style);
                }
            }
            else
            {
                // Straight line at the target coordinates
                foreach (LineCoordinates line in lines)
                {
                    args.DrawingSession.DrawLine(line.X + 0.5f, line.Y, line.X + 0.5f, line.Y + line.Height, stroke);
                }
            }

            // Breakpoints
            IReadOnlyList<Rect> breakpoints = BreakpointLinesCoordinates.Values.ToArray();
            foreach (Rect rect in breakpoints)
            {
                args.DrawingSession.FillRoundedRectangle(rect, 2, 2, Colors.DimGray);
                args.DrawingSession.FillRoundedRectangle(new Rect
                {
                    Height = rect.Height - 2,
                    Width = rect.Width - 2,
                    X = rect.X + 1,
                    Y = rect.Y + 1
                }, 2, 2, "#FF762C2C".ToColor());
            }
        }

        // Updates the clip size of the bracket guides container
        private void BracketsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BracketsClip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
        }

        #endregion

        #region Control characters rendering

        // Synchronization semaphore for the control character overlays
        private readonly SemaphoreSlim ControlCharactersSemaphore = new SemaphoreSlim(1);

        // The timestamp of the last redraw of the control characters
        private DateTime _ControlCharactersRenderingTimestamp = DateTime.MinValue;

        // The minimum delay between each redraw of the control characters
        private const int MinimumControlCharactersRenderingInterval = 400;

        // The queue of arguments to render the control characters after a delay
        private int _PendingUpdates;

        // Indicates whether or not to display the control characters in the IDE
        private bool _WhitespacesRenderingEnabled;

        // Gets the list of whitespace characters that need to be rendered
        private IReadOnlyCollection<CharacterWithArea> _CharactersToRender = new List<CharacterWithArea>();

        // Renders the whitespace characters indicators
        private void WhitespacesCanvas_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            IReadOnlyCollection<CharacterWithArea> items = _CharactersToRender;
            foreach (CharacterWithArea c in items)
            {
                if (c.Character == ' ')
                {
                    Rect dot = new Rect
                    {
                        Height = 2,
                        Width = 2,
                        X = c.Area.Left + (c.Area.Right - c.Area.Left) / 2 + 3,
                        Y = c.Area.Top + (c.Area.Bottom - c.Area.Top) / 2 - 1
                    };
                    args.DrawingSession.FillRectangle(dot, Colors.DimGray);
                }
                else
                {
                    double width = c.Area.Right - c.Area.Left;
                    if (width < 12)
                    {
                        // Small dot at the center
                        Rect dot = new Rect
                        {
                            Height = 2,
                            Width = 2,
                            X = c.Area.Left + width / 2 + 5,
                            Y = c.Area.Top + (c.Area.Bottom - c.Area.Top) / 2 - 1
                        };
                        args.DrawingSession.FillRectangle(dot, Colors.DimGray);
                    }
                    else
                    {
                        // Arrow indicator
                        float
                            x = (float)(c.Area.Left + width / 2),
                            y = (float)(c.Area.Top + (c.Area.Bottom - c.Area.Top) / 2 - 2);
                        int length = width < 28 ? 8 : 12;
                        args.DrawingSession.DrawLine(x, y + 2, x + length, y + 2, Colors.DimGray);
                        args.DrawingSession.DrawLine(x + length - 2, y, x + length, y + 2, Colors.DimGray);
                        args.DrawingSession.DrawLine(x + length - 2, y + 4, x + length, y + 2, Colors.DimGray);
                    }
                }
            }
        }

        /// <summary>
        /// Renders the current control characters
        /// </summary>
        private async void RenderControlCharacters()
        {
            // Private function to render the characters
            IReadOnlyCollection<CharacterWithArea> RenderControlCharactersCode()
            {
                // Edge case
                EditBox.Document.GetText(TextGetOptions.None, out string code);
                if (code.Length < 2)
                {
                    return new List<CharacterWithArea>();
                }

                // Find the target characters
                List<CharacterWithArea> characters = new List<CharacterWithArea>();
                for (int i = 0; i < code.Length; i++)
                {
                    char c = code[i];
                    if (c == ' ')
                    {
                        ITextRange range = EditBox.Document.GetRange(i, i + 1);
                        range.GetRect(PointOptions.Transform, out Rect area, out _);
                        characters.Add(new CharacterWithArea(area, c));
                    }
                    else if (c == '\t')
                    {
                        ITextRange range = EditBox.Document.GetRange(i, i + 1);
                        range.GetRect(PointOptions.Transform, out Rect area, out _);
                        characters.Add(new CharacterWithArea(area, c));
                    }
                }
                return characters;
            }

            // Lock and add the delayed handler
            await ControlCharactersSemaphore.WaitAsync();
            _PendingUpdates++;
            Task.Delay(MinimumControlCharactersRenderingInterval).ContinueWith(async t =>
            {
                // Lock again and retrieve the current argument
                await ControlCharactersSemaphore.WaitAsync();
                try
                {
                    if (_PendingUpdates == 0) return;
                    if (DateTime.Now.Subtract(_ControlCharactersRenderingTimestamp).TotalMilliseconds < MinimumControlCharactersRenderingInterval &&
                        _PendingUpdates > 0) return; // Skip if another handler was executed less than half a second ago
                    _ControlCharactersRenderingTimestamp = DateTime.Now;
                    _CharactersToRender = RenderControlCharactersCode(); // Render the control characters
                    WhitespacesCanvas.Invalidate();
                }
                catch
                {
                    // Sorry, can't crash here
                }
                finally
                {
                    ControlCharactersSemaphore.Release();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
            ControlCharactersSemaphore.Release();
        }

        /// <summary>
        /// Clears the current control characters rendered on screen
        /// </summary>
        private async void ClearControlCharacters()
        {
            await ControlCharactersSemaphore.WaitAsync();
            _PendingUpdates = 0;
            _CharactersToRender = new List<CharacterWithArea>();
            _ControlCharactersRenderingTimestamp = DateTime.Now;
            ControlCharactersSemaphore.Release();
            WhitespacesCanvas.Invalidate();
        }

        // Adjusts the size of the whitespace overlays canvas
        private void EditBox_OnTextSizeChanged(object sender, SizeChangedEventArgs e)
        {
            WhitespacesCanvas.Height = BracketGuidesCanvas.Height = e.NewSize.Height + _Top + 20;
            WhitespacesCanvas.Width = BracketGuidesCanvas.Width = e.NewSize.Width;
        }

        // Updates the clip size of the control character overlays container
        private void WhitespaceParentCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ControlCharactersClip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
        }

        #endregion

        #region Syntax highlight

        // Gets the backup of the text in the IDE
        private string _PreviousText;

        // Gets the previous length of the text selection
        private int _PreviousSelectionLength;

        // Updates the syntax highlight and some UI overlays whenever the text selection changes
        private void EditBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            /* ====================
             * Syntax highlight
             * ================= */

            // Get the current text and backup the current index
            EditBox.Document.GetText(TextGetOptions.None, out string text);
            int start = EditBox.Document.Selection.StartPosition;

            // Single character entered
            bool textChanged = false;
            if (text.Length == _PreviousText.Length + 1 ||                                                              // Single character added
                _PreviousSelectionLength > 0 && EditBox.Document.Selection.Length == 0 && !_PreviousText.Equals(text))  // Long text selected replaced with a single char
            {
                // Unsubscribe from the text events and batch the updates
                EditBox.SelectionChanged -= EditBox_OnSelectionChanged;
                EditBox.TextChanged -= EditBox_OnTextChanged;

                try
                {
                    // Get the last character and apply the right color
                    ITextRange range = EditBox.Document.GetRange(start - 1, start);
                    char character = range.Character;
                    if (!(start > 1 && text[start - 2] == '(' && character == '[') && // Allow to directly open a loop inside a function
                        (!Brainf_ckInterpreter.CheckSourceSyntax(_PreviousText).Valid || character != '[' && character != '(' && character != '\r'))
                    {
                        // Avoid applying the syntax highlight twice
                        range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar(character);
                    }
                    else // Syntax highlight and autocompletion for the [, \r and ( characters
                    {
                        // Calculate the current indentation depth
                        string trailer = text.Substring(0, range.StartPosition);
                        int indents = trailer.Count(c => c == '[') - trailer.Count(c => c == ']');
                        string tabs = '\t'.Repeat(indents);

                        // Open [ bracket
                        if (character == '[')
                        {
                            // Get the current settings
                            bool autoFormat = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.AutoIndentBrackets));
                            int formatMode = autoFormat
                                ? AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.BracketsStyle))
                                : default;

                            // Edge case: the user was already on an empty and indented line when opening the bracket
                            bool edge = false;
                            int lastCr = trailer.LastIndexOf('\r');
                            if (lastCr != -1)
                            {
                                // Autocomplete without the first blank line
                                string lastLine = trailer.Substring(lastCr, trailer.Length - lastCr);
                                if (lastLine.Skip(1).All(c => c == '\t') && lastLine.Length == indents + 1)
                                {
                                    EditBox.Document.Selection.TypeText(autoFormat ? $"\r{tabs}\t\r{tabs}]" : "]");
                                    edge = true;
                                }
                            }

                            // Edge case: first line in the document
                            if (range.StartPosition == 0)
                            {
                                EditBox.Document.Selection.TypeText(autoFormat ? $"\r{tabs}\t\r{tabs}]" : "]");
                                edge = true;
                            }

                            // Default autocomplete: new line and [ ] brackets
                            if (!edge)
                            {
                                if (autoFormat)
                                {
                                    if (formatMode == 0) // New line
                                    {
                                        range.Delete(TextRangeUnit.Character, 1);
                                        EditBox.Document.Selection.TypeText($"\r{tabs}[\r{tabs}\t\r{tabs}]");
                                    }
                                    else EditBox.Document.Selection.TypeText($"\r{tabs}\t\r{tabs}]");
                                }
                                else EditBox.Document.Selection.TypeText("]");
                            }

                            // Apply the right color and move the selection at the center of the brackets
                            ITextRange bracketsRange = EditBox.Document.GetRange(start - 1, EditBox.Document.Selection.EndPosition);
                            bracketsRange.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar('[');
                            EditBox.Document.Selection.Move(TextRangeUnit.Character, -(autoFormat ? indents + 2 : 1));
                            DrawLineNumbers();
                            textChanged = true;
                        }
                        else if (character == '\r')
                        {
                            // New line, tabs needed
                            if (tabs.Length > 0) EditBox.Document.Selection.TypeText(tabs);
                            DrawLineNumbers();
                            textChanged = true;
                        }
                        else if (character == '(')
                        {
                            // Function definition
                            EditBox.Document.Selection.TypeText(")");
                            ITextRange bracketsRange = EditBox.Document.GetRange(start - 1, EditBox.Document.Selection.EndPosition);
                            bracketsRange.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar('(');
                            EditBox.Document.Selection.Move(TextRangeUnit.Character, -1);
                            textChanged = true;
                        }
                    }
                }
                catch
                {
                    // This must never crash
                }

                // Restore the event handlers
                if (!ViewModel.DisableUndoGroupManagement) EditBox.Document.EndUndoGroup();
                EditBox.SelectionChanged += EditBox_OnSelectionChanged;
                EditBox.TextChanged += EditBox_OnTextChanged;
            }

            // Refresh the current text if needed
            if (textChanged)
            {
                EditBox.Document.GetText(TextGetOptions.None, out text);
            }

            // Display the text updates
            _PreviousText = text;
            _PreviousSelectionLength = EditBox.Document.Selection.Length;

            // Update the bracket guides
            if (textChanged)
            {
                DrawBracketGuides(text, false).ContinueWith(t =>
                {
                    if (t.Result.Status != AsyncOperationStatus.RunToCompletion) return;
                    ViewModel.UpdateIndentationInfo(t.Result.Result).Forget();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            // Update the cursor indicators
            UpdateCursorRectangleAndIndicatorUI();

            // Notify the UI
            ViewModel.SendMessages(text);

            // Scroll if needed
            EditBox.TryScrollToSelection();
        }

        /// <summary>
        /// Updates the position of the rectangle that highlights the selected line and the custom cursor
        /// </summary>
        private void UpdateCursorRectangleAndIndicatorUI()
        {
            // Move the cursor to the right position
            Point selectionOffset = EditBox.ActualSelectionVerticalOffset;
            CursorTransform.Y = selectionOffset.Y;
            CursorTransform.X = selectionOffset.X + 4;

            // Update the visibility and the position of the cursor
            CursorBorder.Visibility = (EditBox.Document.Selection.Length.Abs() == 0).ToVisibility();
            CursorBorderTransform.Y = selectionOffset.Y;
        }

        // Hides the custom cursor and highlights the line indicator
        private void EditBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            CursorAnimation.Stop();
            CursorRectangle.Visibility = Visibility.Collapsed;
            VisualStateManager.GoToState(this, "Focused", false);
        }

        // Shows and animates the custom cursor and reduces the opacity of the selected line rectangle
        private void EditBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            CursorRectangle.Visibility = Visibility.Visible;
            CursorAnimation.Begin();
            VisualStateManager.GoToState(this, "Default", false);
        }

        // Manually handles a paste operations within the IDE
        private void EditBox_OnPaste(object sender, TextControlPasteEventArgs e)
        {
            e.Handled = true;
            TryPasteFromClipboard();
        }

        // Handles a request by the user to perform a clipboard operation
        private void HandleClipboardOperationRequest(ClipboardOperation operation)
        {
            if (operation == ClipboardOperation.Paste) TryPasteFromClipboard();
            else
            {
                EditBox.Document.Selection.GetText(TextGetOptions.None, out string selection);
                selection.TryCopyToClipboard();
                if (operation == ClipboardOperation.Cut) EditBox.Document.Selection.SetText(TextSetOptions.None, string.Empty);
            }
            EditBox.Focus(FocusState.Programmatic);
        }

        // Tries to get some text to paste from the clipboard
        private async void TryPasteFromClipboard()
        {
            // Retrieve the contents as plain text
            var (text, format) = await ClipboardHelper.TryGetTextAsync();
            if (format?.Equals(StandardDataFormats.Rtf) == true)
            {
                RichEditBox provider = new RichEditBox();
                provider.Document.SetText(TextSetOptions.FormatRtf, text);
                provider.Document.GetText(TextGetOptions.None, out text);
            }
            if (text == null) return;
            LoadCode(text, false);
        }

        /// <summary>
        /// Manually loads some code into the IDE
        /// </summary>
        /// <param name="code">The code to load</param>
        /// <param name="overwrite">If true, the whole document will be replaced with the new code</param>
        /// <param name="offset">The optional cursor offset to apply after loading the code (in case it's a code snippet)</param>
        private async void LoadCode(string code, bool overwrite, int? offset = null)
        {
            // Disable the handlers
            EditBox.SelectionChanged -= EditBox_OnSelectionChanged;
            EditBox.TextChanged -= EditBox_OnTextChanged;
            if (!overwrite) EditBox.Document.BeginUndoGroup();

            // Fade out the UI if needed
            bool fade = !overwrite && code.Length > 1000;
            if (fade)
            {
                Messenger.Default.Send(new AppLoadingStatusChangedMessage(true));
                await Task.Delay(250);
                ClearControlCharacters();
            }

            try
            {
                // Adjust the input text
                code = code.Replace("\n", "");

                // Paste the text and get the target range
                int start, end;
                SolidColorBrush selectionBackup = null;
                if (overwrite)
                {
                    // Load a stream with the new text to also reset the undo stack
                    await EditBox.LoadTextAsync(code);
                    ApplyCustomTabSpacing();
                    start = 0;
                    end = code.Length;
                }
                else
                {
                    // Autoindent if needed
                    start = EditBox.Document.Selection.StartPosition;
                    if (offset != null)
                    {
                        // Adjust the formatting style, if needed
                        if (AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.BracketsStyle)) == 1)
                        {
                            int newlines = code.Substring(0, offset.Value).Split("\r[\r").Length - 1;
                            if (newlines > 0) offset = offset.Value - newlines;
                            code = code.Replace("\r[\r", "[\r");
                        }

                        // Format
                        EditBox.Document.GetText(TextGetOptions.None, out string text);
                        string trailer = text.Substring(0, start);
                        int indents = trailer.Count(c => c == '[') - trailer.Count(c => c == ']');
                        string tabs = '\t'.Repeat(indents);
                        StringBuilder builder = new StringBuilder();
                        int cursor = offset.Value;
                        foreach ((char c, int i) in code.Select((c, i) => (c, i)))
                        {
                            if (c == '\r')
                            {
                                if (i <= code.Length - 2 && code[i + 1] == ']' && indents >= 1)
                                    tabs = '\t'.Repeat(--indents);
                                builder.Append($"{c}{tabs}");
                                if (offset > i) cursor += indents;
                            }
                            else if (c == '[')
                            {
                                builder.Append(c);
                                indents++;
                                tabs += "\t";
                            }
                            else if (c == ']')
                            {
                                if (indents >= 1 && !(i > 0 && code[i - 1] == '\r')) tabs = '\t'.Repeat(--indents);
                                builder.Append(c);
                            }
                            else builder.Append(c);
                        }
                        code = builder.ToString();
                        offset = cursor; // Updated target offset (considering the added tabs)
                    }

                    // Paste the text in the current selection
                    EditBox.Document.Selection.SetText(TextSetOptions.None, code);
                    selectionBackup = EditBox.SelectionHighlightColor;
                    EditBox.SelectionHighlightColor = new SolidColorBrush(Colors.Transparent);
                    end = EditBox.Document.Selection.EndPosition;
                }

                // Highlight the new text
                int count = 0;
                for (int i = start; i < end; i++)
                {
                    char test = code[count++];
                    if (test < 33 || test > 126 && test < 161) continue;
                    ITextRange range = EditBox.Document.GetRange(i, i + 1);
                    char c = range.Character;
                    range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar(c);
                }

                // Set the right selection position
                if (overwrite) EditBox.Document.Selection.SetRange(0, 0);
                else if (offset == null) EditBox.Document.Selection.StartPosition = end;
                else EditBox.Document.Selection.StartPosition = EditBox.Document.Selection.EndPosition = start + offset.Value;

                // Refresh the UI
                if (overwrite) ViewModel.DiffStatusSource.Clear();
                else
                {
                    EditBox.Document.EndUndoGroup();
                    EditBox.SelectionHighlightColor = selectionBackup;
                }
                EditBox.Document.GetText(TextGetOptions.None, out code);
                _PreviousText = code;
                _PreviousSelectionLength = EditBox.Document.Selection.Length;
                DrawLineNumbers();
                DrawBracketGuides(code, false).ContinueWith(t =>
                {
                    if (t.Result.Status != AsyncOperationStatus.RunToCompletion) return;
                    ViewModel.UpdateIndentationInfo(t.Result.Result).Forget();
                }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
                if (_WhitespacesRenderingEnabled) RenderControlCharacters();
                ViewModel.UpdateGitDiffStatus(code).Forget();
                ViewModel.SendMessages(code);
                ViewModel.UpdateCanUndoRedoStatus();
                UpdateCursorRectangleAndIndicatorUI();
                if (!overwrite) RemoveUnvalidatedBreakpointsAsync(code).Forget();
            }
            catch
            {
                // Can't crash here, have things to do, people to see
            }

            // Restore the handlers
            EditBox.SelectionChanged += EditBox_OnSelectionChanged;
            EditBox.TextChanged += EditBox_OnTextChanged;

            // Restore the UI if needed
            if (fade)
            {
                await Task.Delay(150);
                Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
            }
        }

        // Updates the clip size of the container of the unfocused text cursor
        private void LineCursorCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LineCursorClip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
            CursorBorder.Width = e.NewSize.Width;
        }

        #endregion

        #region Breakpoints

        /// <summary>
        /// Gets the collection of current visualized breakpoints and their respective line numbers
        /// </summary>
        private readonly Dictionary<int, Tuple<Ellipse, Guid>> BreakpointsInfo = new Dictionary<int, Tuple<Ellipse, Guid>>();

        // Clears the breakpoints from the UI and their info
        private void ClearBreakpoints()
        {
            BreakpointsInfo.Clear();
            BreakpointLinesCoordinates.Clear();
            BreakpointsCanvas.Children.Clear();
        }

        /// <summary>
        /// Removes the invalid breakpoints after the text is edited
        /// </summary>
        /// <param name="text">The current text</param>
        private async Task RemoveUnvalidatedBreakpointsAsync([NotNull] string text)
        {
            // Find the invalid breakpoints
            IReadOnlyList<int> pending = await Task.Run(() => BreakpointsInfo.Keys.Where(line =>
                line < 2 ||                                                         // Breakpoints not allowed in the first two lines
                line > text.Length ||                                               // Breakpoints fallen out of range (eg. when deleting code lines)
                !text.GetLine(line).Any(Brainf_ckInterpreter.Operators.Contains)    // Breakpoints on lines with no longer at least one operator
                ).ToArray());

            // Remove the target breakpoints
            foreach (int target in pending)
            {
                if (BreakpointsInfo.TryGetValue(target, out Tuple<Ellipse, Guid> previous))
                {
                    // Remove the previous breakpoint
                    BreakpointsCanvas.Children.Remove(previous.Item1);
                    BreakpointsInfo.Remove(target);
                    BreakpointLinesCoordinates.Remove(previous.Item2);
                }
            }
            BracketGuidesCanvas.Invalidate();
        }

        // Shows the breakpoints from an input list of lines
        private void RestoreBreakpoints(IReadOnlyCollection<int> lines)
        {
            // Remove the previous breakpoints
            ClearBreakpoints();

            // Get the text
            EditBox.Document.GetText(TextGetOptions.None, out string code);

            // Get the actual positions for each line start
            IReadOnlyCollection<int> indexes = code.FindLineIndexes(lines).Select(i => i + 1).ToArray();

            // Draw the breakpoints again
            foreach (int i in indexes)
            {
                ITextRange range = EditBox.Document.GetRange(i, i);
                range.GetRect(PointOptions.Transform, out Rect rect, out _);
                AddSingleBreakpoint(code, i, rect.Top);
            }
        }

        // Adds/removes a breakpoint when tapping on the left side bar
        private void BreakpointsCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Get the text point corresponding to the tap position
            Point
                tap = e.GetPosition(this),
                point = new Point(0, tap.Y -                                // Original tap Y offset
                                     (_Top + 10) +                          // Offset to the upper margin of the window
                                     EditBox.VerticalScrollViewerOffset);   // Take the current vertical scrolling into account

            // Get the range aligned to the left edge of the tapped line
            ITextRange range = EditBox.Document.GetRangeFromPoint(point, PointOptions.ClientCoordinates);
            range.GetRect(PointOptions.Transform, out Rect line, out _);

            // Get the line number
            EditBox.Document.GetText(TextGetOptions.None, out string text);

            // Add the breakpoint
            AddSingleBreakpoint(text, range.StartPosition, line.Top);
            Messenger.Default.Send(new DebugStatusChangedMessage(BreakpointsInfo.Keys.Count > 0));
        }

        // The guid to synchronize the invalid breakpoint messages being sent with a delay
        private Guid _InvalidBreakpointMessageID;

        /// <summary>
        /// Calculates the visual coordinates and info for a breakpoint to insert at a given line
        /// </summary>
        /// <param name="text">The source text</param>
        /// <param name="index">The breakpoint initial index</param>
        private (double X, double Y, double Width) CalculateBreakpointCoordinates([NotNull] string text, int index)
        {
            // Get the target line coordinates
            int first = 0, last = -1;
            bool found = false, space = false;
            for (int i = index; i < text.Length; i++)
            {
                if (Brainf_ckInterpreter.Operators.Contains(text[i]))
                {
                    if (!found)
                    {
                        found = true;
                        first = i;
                    }
                }
                else if (found)
                {
                    // Store the final index and break
                    last = i;

                    // Check if there's an available space to extend the breakpoint indicator width
                    if (text[i] == ' ' || text[i] == '\t' || text[i] == '\r') space = true;
                    break;
                }
            }

            // Get the initial and ending range
            ITextRange range = EditBox.Document.GetRange(first, first);
            range.GetRect(PointOptions.Transform, out Rect open, out _);
            range = EditBox.Document.GetRange(last, last);
            range.GetRect(PointOptions.Transform, out Rect close, out _);
            return (open.X + 2, open.Top, close.Right - open.Left + (space ? 3 : 2));
        }

        /// <summary>
        /// Refreshes the UI of the breakpoint overlays
        /// </summary>
        /// <param name="text">The current IDE text</param>
        private void RefreshBreakpointsUI([NotNull] string text)
        {
            KeyValuePair<int, Tuple<Ellipse, Guid>>[] pairs = BreakpointsInfo.ToArray();

            // Get the actual positions for each line start
            IReadOnlyList<int> indexes = text.FindLineIndexes(pairs.Select(p => p.Key - 1).ToArray());
            for (int i = 0; i < pairs.Length; i++)
            {
                KeyValuePair<int, Tuple<Ellipse, Guid>> pair = pairs[i];
                (double x, double y, double width) = CalculateBreakpointCoordinates(text, indexes[i]);
                Rect rect = new Rect(x, _Top + 10 + y + _BreakpointsLineOffset, width, _ApproximateLineHeight);
                BreakpointLinesCoordinates[pair.Value.Item2] = rect;
            }
            BracketGuidesCanvas.Invalidate();
        }

        private readonly IDictionary<Guid, Rect> BreakpointLinesCoordinates = new Dictionary<Guid, Rect>();

        /// <summary>
        /// Adds a single breakpoint to the UI and the backup list
        /// </summary>
        /// <param name="text">The current text, if available</param>
        /// <param name="start">The start index for the breakpoint</param>
        /// <param name="offset">The vertical offset of the selected line</param>
        private void AddSingleBreakpoint([CanBeNull] string text, int start, double offset)
        {
            // Get the target line
            if (text == null) EditBox.Document.GetText(TextGetOptions.None, out text);
            Coordinate coordinate = text.FindCoordinates(start);
            if (coordinate.Y == 1 || // Can't place a breakpoint on the first line
                !text.GetLine(coordinate.Y).Any(Brainf_ckInterpreter.Operators.Contains)) // Invalid line, no operators here
            {
                // Send a message to signal the breakpoint can't be placed here
                Guid id = Guid.NewGuid();
                _InvalidBreakpointMessageID = id;
                Messenger.Default.Send(new BreakpointErrorStatusChangedMessage(false));
                Task.Delay(5000).ContinueWith(t =>
                {
                    // Only send the second message if no other messages were sent before this moment
                    if (!_InvalidBreakpointMessageID.Equals(id)) return;
                    Messenger.Default.Send(new BreakpointErrorStatusChangedMessage(true));
                }, TaskScheduler.FromCurrentSynchronizationContext());
                return;
            }

            // Setup the breakpoint
            if (BreakpointsInfo.TryGetValue(coordinate.Y, out Tuple<Ellipse, Guid> previous))
            {
                // Remove the previous breakpoint
                BreakpointsCanvas.Children.Remove(previous.Item1);
                BreakpointsInfo.Remove(coordinate.Y);
                BreakpointLinesCoordinates.Remove(previous.Item2);
                BracketGuidesCanvas.Invalidate();
            }
            else
            {
                // Breakpoint ellipse
                Ellipse ellipse = new Ellipse
                {
                    Width = 14,
                    Height = 14,
                    Fill = XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("BreakpointFillBrush"),
                    Stroke = XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("BreakpointBorderBrush"),
                    StrokeThickness = 1,
                    RenderTransform = new TranslateTransform
                    {
                        X = 3,
                        Y = _Top + 10 + offset
                    }
                };
                BreakpointsCanvas.Children.Add(ellipse);
                ellipse.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y);

                // Line highlight
                (double x, _, double width) = CalculateBreakpointCoordinates(text, start);
                Rect rect = new Rect(x, _Top + 10 + offset + _BreakpointsLineOffset, width, _ApproximateLineHeight);
                Guid guid = Guid.NewGuid();
                BreakpointLinesCoordinates.Add(guid, rect);
                BracketGuidesCanvas.Invalidate();

                // Store the info
                BreakpointsInfo.Add(coordinate.Y, Tuple.Create(ellipse, guid));
            }
        }

        #endregion

        // Begins a new undo group when the user presses a keyboard key (before the text is actually changed)
        private void EditBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            EditBox.Document.BeginUndoGroup();
            if (e.Key == VirtualKey.Tab)
            {
                // Setup
                EditBox.Document.BatchDisplayUpdates();
                EditBox.SelectionChanged -= EditBox_OnSelectionChanged;
                EditBox.TextChanged -= EditBox_OnTextChanged;

                // Handle the special action
                if (EditBox.Document.Selection.Length.Abs() < 2) EditBox.Document.Selection.TypeText("\t");
                else if (Window.Current.CoreWindow.GetKeyState(VirtualKey.LeftShift).HasFlag(CoreVirtualKeyStates.Down))
                {
                    // Shift back
                    EditBox.Document.Selection.GetText(TextGetOptions.None, out string text);
                    string[] lines = text.Split('\r');
                    (int start, int end) = EditBox.Document.Selection.GetAbsPositions();
                    int characters = 0, removed = 0;
                    foreach (string line in lines)
                    {
                        if (line.StartsWith('\t'))
                        {
                            EditBox.Document.Selection.EndPosition = start + characters + 1;
                            EditBox.Document.Selection.StartPosition = EditBox.Document.Selection.EndPosition - 1;
                            EditBox.Document.Selection.SetText(TextSetOptions.None, string.Empty);
                            characters += line.Length;
                            removed++;
                        }
                        else characters += line.Length + 1;
                    }
                    EditBox.Document.Selection.StartPosition = start;
                    EditBox.Document.Selection.EndPosition = end - removed;
                }
                else
                {
                    // Shift forward
                    EditBox.Document.Selection.GetText(TextGetOptions.None, out string text);
                    string[] lines = text.Split('\r');
                    lines = lines.Take(lines.Length - 1).ToArray();
                    (int start, int end) = EditBox.Document.Selection.GetAbsPositions();
                    int characters = 0, added = 0;
                    foreach (string line in lines)
                    {
                        EditBox.Document.Selection.StartPosition = EditBox.Document.Selection.EndPosition = start + characters;
                        EditBox.Document.Selection.TypeText("\t");
                        characters += line.Length + 2;
                        added++;
                    }
                    EditBox.Document.Selection.StartPosition = start;
                    EditBox.Document.Selection.EndPosition = end + added;
                }

                // Restore the UI
                EditBox.Document.ApplyDisplayUpdates();
                EditBox.Document.EndUndoGroup();
                EditBox.SelectionChanged += EditBox_OnSelectionChanged;
                EditBox.TextChanged += EditBox_OnTextChanged;
                e.Handled = true;

                // Apply UI updates
                EditBox.Document.GetText(TextGetOptions.None, out string code);
                _PreviousText = code;
                _PreviousSelectionLength = EditBox.Document.Selection.Length;
                DrawBracketGuides(code, false).Forget();
                ViewModel.UpdateGitDiffStatus(code).Forget();
                ViewModel.UpdateCanUndoRedoStatus();
                RefreshBreakpointsUI(code);
                RenderControlCharacters();
            }
        }

        // Adjusts the vertical scaling of the indentation indicators
        private void AdjustIndentationIndicatorsVerticalStretch(Size newSize)
        {
            int count = ViewModel.Source.Count;
            if (count < 3) IndentationInfoList.SetVisualScale(null, 1, null);
            else
            {
                string lines = '\n'.Repeat(count - 1);
                Size size = lines.MeasureText(15, LineBlock.FontFamily);
                IndentationInfoList.SetVisualScale(null, (float)(size.Height / newSize.Height), null);
            }
        }

        // Adjusts the stretch of the indentation indicators whenever the size of the indicators changes
        private void IndentationInfoList_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustIndentationIndicatorsVerticalStretch(e.NewSize);
        }

        // Adjusts the vertical scaling of the git diff indicators
        private void AdjustGitDiffIndicatorsVerticalStretch(Size newSize)
        {
            int count = ViewModel.DiffStatusSource.Count;
            if (count < 3) GitDiffListView.SetVisualScale(null, 1, null);
            else
            {
                string lines = '\n'.Repeat(count - 1);
                Size size = lines.MeasureText(15, LineBlock.FontFamily);
                GitDiffListView.SetVisualScale(null, (float)(size.Height / newSize.Height), null);
            }
        }

        // Adjusts the vertical scaling of the git diff indicators whenever the size of the indicators list changes
        private void GitDiffListView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustGitDiffIndicatorsVerticalStretch(e.NewSize);
        }

        // Refreshes the size of the indicators
        private void RefreshListViewsStretch()
        {
            AdjustIndentationIndicatorsVerticalStretch(new Size(IndentationInfoList.ActualWidth, IndentationInfoList.ActualHeight));
            AdjustGitDiffIndicatorsVerticalStretch(new Size(GitDiffListView.ActualWidth, GitDiffListView.ActualHeight));
        }

        #region Breakpoints context menu

        // Shows the MenuFlyout when using a mouse or a pen
        private void BreakpointsCanvas_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch || BreakpointsInfo.Count == 0) return;
            ShowMenuFlyout(e.GetPosition(this));
        }

        // Shows the MenuFlyout when the input device is a touch screen
        private void BreakpointsCanvas_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch ||
                e.HoldingState != HoldingState.Started || BreakpointsInfo.Count == 0) return;
            ShowMenuFlyout(e.GetPosition(this));
        }

        // Shows the MenuFlyout at the right position
        private void ShowMenuFlyout(Point offset)
        {
            // Get the custom MenuFlyout
            MenuFlyout menuFlyout = MenuFlyoutHelper.PrepareBreakpointsMenuFlyout(
            () =>
            {
                ClearBreakpoints();
                BracketGuidesCanvas.Invalidate();
                ViewModel.SignalBreakpointsDeleted();
            });
            menuFlyout.ShowAt(this, offset);
        }

        #endregion

        // Overrides the default context menu on right click
        private void EditBox_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
            FullCodeSnippetsBrowserFlyout browser = new FullCodeSnippetsBrowserFlyout(EditBox.Document)
            {
                Height = 48 * 6 + 43, // Ugly hack (height of a snippet template by number of available templates)
                Width = 220
            };
            FlyoutManager.Instance.ShowCustomContextFlyout(browser, EditBox, 
                margin: new Point(e.CursorLeft - 70, e.CursorTop), invertAnimation: true).Forget();
        }
    }
}
