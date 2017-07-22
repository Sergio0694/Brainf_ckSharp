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
using Windows.UI.Input;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.EventArgs;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.DataModels.Misc.CharactersInfo;
using Brainf_ck_sharp_UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Messages.IDEStatus;
using Brainf_ck_sharp_UWP.Messages.UI;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
using Brainf_ck_sharp_UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Microsoft.Graphics.Canvas.UI.Xaml;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers;

namespace Brainf_ck_sharp_UWP.Views
{
    public sealed partial class IDEView : UserControl
    {
        public IDEView()
        {
            Loaded += IDEView_Loaded;
            this.InitializeComponent();
            ApplyUITheme();
            ApplyCustomTabSpacing();
            DataContext = new IDEViewModel(EditBox.Document, PickSaveNameAsync, () => BreakpointsInfo.Keys);
            ViewModel.PlayRequested += ViewModel_PlayRequested;
            ViewModel.LoadedCodeChanged += (_, e) =>
            {
                // Load the code
                LoadCode(e.Code, true);
                if (e.Breakpoints == null) ClearBreakpoints();
                else RestoreBreakpoints(BitHelper.Expand(e.Breakpoints));
                Messenger.Default.Send(new DebugStatusChangedMessage(BreakpointsInfo.Keys.Count > 0));

                // Restore the UI
                Task.Delay(250).ContinueWith(t =>
                {
                    Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
                }, TaskScheduler.FromCurrentSynchronizationContext());
            };
            ViewModel.TextCleared += (_, e) =>
            {
                EditBox.ResetTextAndUndoStack();
                ApplyCustomTabSpacing();
                ClearBreakpoints();
                Messenger.Default.Send(new DebugStatusChangedMessage(BreakpointsInfo.Keys.Count > 0));
            };
            ViewModel.CharInsertionRequested += ViewModel_CharInsertionRequested;
            ViewModel.NewLineInsertionRequested += ViewModel_NewLineInsertionRequested;
            EditBox.Document.GetText(TextGetOptions.None, out String text);
            _PreviousText = text;
            _WhitespacesRenderingEnabled = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.RenderWhitespaces));
            Messenger.Default.Register<IDESettingsChangedMessage>(this, ApplyIDESettings);
        }

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
                Canvas.SetZIndex(CursorBorder, 1);
            }
            else
            {
                CursorBorder.BorderThickness = new Thickness(0);
                CursorBorder.Background = Brainf_ckFormatterHelper.Instance.CurrentTheme.LineHighlightColor.ToBrush();
                Canvas.SetZIndex(BracketsParentGrid, 1);
                Canvas.SetZIndex(CursorBorder, 0);
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
                if (!message.ThemeChanged) DrawBracketGuides(null, true).Forget();
            }

            // Update the render whitespaces setting
            if (message.WhitespacesChanged)
            {
                bool renderWhitespaces = _WhitespacesRenderingEnabled = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.RenderWhitespaces));
                if (renderWhitespaces) RenderControlCharacters();
                else ClearControlCharacters();
            }

            // Update the current UI theme
            if (message.ThemeChanged)
            {
                // Update the theme
                Brainf_ckFormatterHelper.Instance.ReloadTheme();

                // Main UI
                ApplyUITheme();

                // Code highlight
                EditBox.Document.GetText(TextGetOptions.None, out String text);
                int count = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    char test = text[count++];
                    if (test < 33 || test > 126 && test < 161) continue;
                    ITextRange range = EditBox.Document.GetRange(i, i + 1);
                    char c = range.Character;
                    range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar(c);
                }

                // Brackets guides
                DrawBracketGuides(text, true).Forget();

                // Release the UI
                Task.Delay(500).ContinueWith(t =>
                {
                    Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
                }, TaskScheduler.FromCurrentSynchronizationContext());
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

        /// <summary>
        /// Prompts the user to select a file name to save the current source code
        /// </summary>
        /// <param name="code">The source code that's being saved</param>
        private async Task<String> PickSaveNameAsync(String code)
        {
            SaveCodePromptFlyout flyout = new SaveCodePromptFlyout(code, null);
            FlyoutResult result = await FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("SaveCode"), 
                flyout, new Thickness(12, 12, 16, 12), FlyoutDisplayMode.ActualHeight);
            return result == FlyoutResult.Confirmed ? flyout.Title : null;
        }

        private void ViewModel_PlayRequested(object sender, PlayRequestedEventArgs e)
        {
            // Get the text and initialize the session
            EditBox.Document.GetText(TextGetOptions.None, out String text);
            Func<InterpreterExecutionSession> factory;
            if (e.Debug)
            {
                // Get the lines with a breakpoint and deconstruct the source in executable chuncks
                IReadOnlyCollection<int> 
                    lines = BreakpointsInfo.Keys.OrderBy(key => key).Select(i => i - 1).ToArray(), // i -1 to move from 1 to 0-based indexes
                    indexes = text.FindIndexes(lines);
                List<String> chuncks = new List<String>();
                int previous = 0;
                foreach (int breakpoint in indexes.Concat(new[] { text.Length - 1 }))
                {
                    chuncks.Add(text.Substring(previous, breakpoint - previous));
                    previous = breakpoint;
                }
                factory = () => Brainf_ckInterpreter.InitializeSession(chuncks, e.Stdin, e.Mode, 64, 1000);
            }
            else factory = () => Brainf_ckInterpreter.InitializeSession(new[] { text }, e.Stdin, e.Mode, 64, 1000);

            // Display the execution popup
            IDERunResultFlyout flyout = new IDERunResultFlyout();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource(e.Debug ? "Debug" : "RunTitle"), flyout, new Thickness()).Forget();
            Task.Delay(100).ContinueWith(t => flyout.ViewModel.InitializeAsync(factory), TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Initializes the scroll events for the code
        private void IDEView_Loaded(object sender, RoutedEventArgs e)
        {
            // Start the cursor animation and subscribe the scroller event
            CursorAnimation.Begin();

            // Setup the expression animations
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

        public IDEViewModel ViewModel => DataContext.To<IDEViewModel>();

        // The current top marginBracketsGrid_SizeChanged
        private double _Top;

        /// <summary>
        /// Adjusts the top margin of the content in the list
        /// </summary>
        /// <param name="height">The desired height</param>
        public void AdjustTopMargin(double height)
        {
            _Top = height;
            LinesGrid.SetVisualOffset(TranslationAxis.Y, (float)(height - 12)); // Adjust the initial offset of the line numbers and indicators
            BreakLinesCanvas.Margin = new Thickness(0, (float)(height + 10), 0, 0);
            IndentationInfoList.SetVisualOffset(TranslationAxis.Y, (float)(height + 10));
            GitDiffListView.SetVisualOffset(TranslationAxis.Y, (float)(height + 10));
            CursorTransform.X = 4;
            BracketsParentGrid.SetVisualOffset(TranslationAxis.Y, (float)height);
            WhitespacesCanvas.SetVisualOffset(TranslationAxis.Y, (float)(height + 10));
            EditBox.Padding = ApiInformationHelper.IsMobileDevice 
                ? new Thickness(4, _Top + 8, 4, 8) 
                : new Thickness(4, _Top + 8, 20, 20);
            EditBox.ScrollBarMargin = new Thickness(0, _Top, 0, 0);
        }

        // Updates the line numbers displayed next to the code box
        private void EditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            DrawLineNumbers();
            DrawBracketGuides(null, false).ContinueWith(t =>
            {
                if (t.Result.Status != AsyncOperationStatus.RunToCompletion) return;
                ViewModel.UpdateIndentationInfo(t.Result.Result).Forget();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            EditBox.Document.GetText(TextGetOptions.None, out String code);
            ViewModel.UpdateGitDiffStatus(ViewModel.LoadedCode?.Code ?? String.Empty, code).Forget();
            ViewModel.UpdateCanUndoRedoStatus();
            RemoveUnvalidatedBreakpointsAsync(code).Forget();

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
            EditBox.Document.GetText(TextGetOptions.None, out String text);
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
        private async Task<AsyncOperationResult<IReadOnlyList<CharacterWithCoordinates>>> DrawBracketGuides(String code, bool force)
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
                BracketGuidesCanvas.Children.Clear();
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
                int i1 = 0;
                foreach (char c in code)
                {
                    if (c == '[' || c == ']')
                    {
                        Coordinate coordinate = code.FindCoordinates(i1);
                        pairs.Add(new CharacterWithCoordinates(coordinate, c));
                    }
                    i1++;
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
            BracketGuidesCanvas.Children.Clear();

            // Draw the guides for each brackets pair
            int i2 = 0;
            foreach (char c in code)
            {
                // Get the index of the corresponding closing bracket (only if they're not on the same line)
                if (_BracketGuidesCts.IsCancellationRequested) break;
                if (c != '[')
                {
                    i2++;
                    continue;
                }
                int height = 0, target = -1;
                bool newLine = false;
                for (int j = i2 + 1; j < code.Length; j++)
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
                    i2++;
                    continue;
                }

                // Get the initial and ending range
                ITextRange range = EditBox.Document.GetRange(i2, i2);
                range.GetRect(PointOptions.Transform, out Rect open, out _);
                range = EditBox.Document.GetRange(target, target);
                range.GetRect(PointOptions.Transform, out Rect close, out _);

                // Render the new line guide
                double top = close.Top - open.Bottom;
                Line guide = new Line
                {
                    Width = 1,
                    StrokeThickness = 1,
                    Height = top,
                    Stroke = new SolidColorBrush(Brainf_ckFormatterHelper.Instance.CurrentTheme.BracketsGuideColor),
                    StrokeDashArray = Brainf_ckFormatterHelper.Instance.CurrentTheme.BracketsGuideStrokesLength.HasValue
                        ? new DoubleCollection { Brainf_ckFormatterHelper.Instance.CurrentTheme.BracketsGuideStrokesLength.Value }
                        : new DoubleCollection(),
                    Y1 = 0,
                    Y2 = top
                };
                guide.SetVisualOffset(TranslationAxis.Y, (float)(_Top + 30 + open.Top));
                guide.SetVisualOffset(TranslationAxis.X, (float)((close.X > open.X ? open.X : close.X) + 6));
                BracketGuidesCanvas.Children.Add(guide);
                i2++;
            }
            BracketGuidesSemaphore.Release();
            return workingSet.Item1;
        }

        #endregion

        #region Control characters rendering

        // Synchronization semaphore for the control character overlays
        private readonly SemaphoreSlim ControlCharactersSemaphore = new SemaphoreSlim(1);

        // The timestamp of the last redraw of the control characters
        private DateTime _ControlCharactersRenderingTimestamp = DateTime.MinValue;

        // The minimum delay between each redraw of the control characters
        private readonly int MinimumControlCharactersRenderingInterval = 600;

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
                        X = c.Area.Left + 5,
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
                EditBox.Document.GetText(TextGetOptions.None, out String code);
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
                        ITextRange range = EditBox.Document.GetRange(i, i);
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
            WhitespacesCanvas.Height = e.NewSize.Height;
            WhitespacesCanvas.Width = e.NewSize.Width;
        }

        // Updates the clip size of the control character overlays container
        private void WhitespaceParentCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ControlCharactersClip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
        }

        #endregion

        // Gets the backup of the text in the IDE
        private String _PreviousText;

        // Updates the syntax highlight and some UI overlays whenever the text selection changes
        private void EditBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            /* ====================
             * Syntax highlight
             * ================= */

            // Get the current text and backup the current index
            EditBox.Document.GetText(TextGetOptions.None, out String text);
            int start = EditBox.Document.Selection.StartPosition;

            // Single character entered
            bool textChanged = false;
            try
            {
                if (text.Length == _PreviousText.Length + 1)
                {
                    // Unsubscribe from the text events and batch the updates
                    EditBox.SelectionChanged -= EditBox_OnSelectionChanged;
                    EditBox.TextChanged -= EditBox_OnTextChanged;

                    // Get the last character and apply the right color
                    ITextRange range = EditBox.Document.GetRange(start - 1, start);
                    range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar(range.Character);

                    // No other work needed for all the operators except the [ bracket
                    if (!Brainf_ckInterpreter.Operators.Where(c => c != '[').Contains(range.Character) &&
                        Brainf_ckInterpreter.CheckSourceSyntax(_PreviousText).Valid) // Skip the autocompletion if the code isn't valid
                    {
                        // Calculate the current indentation depth
                        String trailer = text.Substring(0, range.StartPosition);
                        int indents = trailer.Count(c => c == '[') - trailer.Count(c => c == ']');
                        String tabs = Enumerable.Range(0, indents).Aggregate(new StringBuilder(), (b, c) =>
                        {
                            b.Append('\t');
                            return b;
                        }).ToString();

                        // Open [ bracket
                        if (range.Character == '[')
                        {
                            // Get the current settings
                            bool autoFormat = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.AutoIndentBrackets));
                            int formatMode = autoFormat
                                ? AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.BracketsStyle))
                                : default(int);

                            // Edge case: the user was already on an empty and indented line when opening the bracket
                            bool edge = false;
                            int lastCr = trailer.LastIndexOf('\r');
                            if (lastCr != -1)
                            {
                                // Autocomplete without the first blank line
                                String lastLine = trailer.Substring(lastCr, trailer.Length - lastCr);
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
                            ITextRange bracketsRange = EditBox.Document.GetRange(start, EditBox.Document.Selection.EndPosition);
                            bracketsRange.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar('[');
                            EditBox.Document.Selection.Move(TextRangeUnit.Character, -(autoFormat ? indents + 2 : 1));
                            DrawLineNumbers();
                            textChanged = true;
                        }
                        else if (range.Character == '\r')
                        {
                            // New line, tabs needed
                            if (tabs.Length > 0) EditBox.Document.Selection.TypeText(tabs);
                            DrawLineNumbers();
                            textChanged = true;
                        }
                    }

                    // Restore the event handlers
                    EditBox.Document.EndUndoGroup();
                    EditBox.SelectionChanged += EditBox_OnSelectionChanged;
                    EditBox.TextChanged += EditBox_OnTextChanged;
                }
            }
            catch
            {
                // This must never crash
            }

            // Refresh the current text if needed
            if (textChanged)
            {
                EditBox.Document.GetText(TextGetOptions.None, out text);
            }

            // Display the text updates
            _PreviousText = text;

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
        private async void EditBox_OnPaste(object sender, TextControlPasteEventArgs e)
        {
            // Retrieve the contents as plain text
            e.Handled = true;
            DataPackageView view = Clipboard.GetContent();
            String text;
            if (view.Contains(StandardDataFormats.Text)) text = await view.GetTextAsync();
            else if (view.Contains(StandardDataFormats.Rtf))
            {
                String rtf = await view.GetRtfAsync();
                RichEditBox provider = new RichEditBox();
                provider.Document.SetText(TextSetOptions.FormatRtf, rtf);
                provider.Document.GetText(TextGetOptions.None, out text);
            }
            else text = null;
            view.ReportOperationCompleted(DataPackageOperation.Copy);
            if (text == null) return;
            LoadCode(text, false);
        }

        /// <summary>
        /// Manually loads some code into the IDE
        /// </summary>
        /// <param name="code">The code to load</param>
        /// <param name="overwrite">If true, the whole document will be replaced with the new code</param>
        private async void LoadCode(String code, bool overwrite)
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
                    // Paste the text in the current selection
                    EditBox.Document.Selection.SetText(TextSetOptions.None, code);
                    selectionBackup = EditBox.SelectionHighlightColor;
                    EditBox.SelectionHighlightColor = new SolidColorBrush(Colors.Transparent);
                    start = EditBox.Document.Selection.StartPosition;
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
                else EditBox.Document.Selection.StartPosition = end;

                // Refresh the UI
                if (overwrite) ViewModel.DiffStatusSource.Clear();
                else
                {
                    EditBox.Document.EndUndoGroup();
                    EditBox.SelectionHighlightColor = selectionBackup;
                }
                EditBox.Document.GetText(TextGetOptions.None, out code);
                _PreviousText = code;
                DrawLineNumbers();
                DrawBracketGuides(code, false).ContinueWith(t =>
                {
                    if (t.Result.Status != AsyncOperationStatus.RunToCompletion) return;
                    ViewModel.UpdateIndentationInfo(t.Result.Result).Forget();
                }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
                if (_WhitespacesRenderingEnabled) RenderControlCharacters();
                ViewModel.UpdateGitDiffStatus(ViewModel.LoadedCode?.Code ?? String.Empty, code).Forget();
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

        // Updates the clip size of the bracket guides container
        private void BracketsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BracketsClip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
        }

        // Updates the clip size of the container of the unfocused text cursor
        private void LineCursorCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LineCursorClip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
            CursorBorder.Width = e.NewSize.Width;
        }

        #region Breakpoints

        /// <summary>
        /// Gets the collection of current visualized breakpoints and their respective line numbers
        /// </summary>
        private readonly Dictionary<int, Tuple<Ellipse, Rectangle>> BreakpointsInfo = new Dictionary<int, Tuple<Ellipse, Rectangle>>();

        // Clears the breakpoints from the UI and their info
        private void ClearBreakpoints()
        {
            BreakpointsInfo.Clear();
            BreakpointsCanvas.Children.Clear();
            BreakLinesCanvas.Children.Clear();
        }

        /// <summary>
        /// Removes the invalid breakpoints after the text is edited
        /// </summary>
        /// <param name="text">The current text</param>
        private async Task RemoveUnvalidatedBreakpointsAsync([NotNull] String text)
        {
            // Find the invalid breakpoints
            IReadOnlyList<int> pending = await Task.Run(() =>
            {
                int[] current = BreakpointsInfo.Keys.ToArray();
                List<int> removable = new List<int>();
                foreach (int line in current)
                {
                    if (line < 2 ||
                        line > text.Length ||
                        !text.GetLine(line).Any(Brainf_ckInterpreter.Operators.Contains))
                    {
                        removable.Add(line);
                    }
                }
                return removable.ToArray();
            });

            // Remove the target breakpoints
            foreach (int target in pending)
            {
                if (BreakpointsInfo.TryGetValue(target, out Tuple<Ellipse, Rectangle> previous))
                {
                    // Remove the previous breakpoint
                    BreakpointsCanvas.Children.Remove(previous.Item1);
                    BreakLinesCanvas.Children.Remove(previous.Item2);
                    BreakpointsInfo.Remove(target);
                }
            }
        }

        // Shows the breakpoints from an input list of lines
        private void RestoreBreakpoints(IReadOnlyCollection<int> lines)
        {
            // Get the text
            EditBox.Document.GetText(TextGetOptions.None, out String code);

            // Get the actual positions for each line start
            IReadOnlyCollection<int> indexes = code.FindIndexes(lines);

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
            EditBox.Document.GetText(TextGetOptions.None, out String text);

            // Add the breakpoint
            AddSingleBreakpoint(text, range.StartPosition, line.Top);
            Messenger.Default.Send(new DebugStatusChangedMessage(BreakpointsInfo.Keys.Count > 0));
        }

        // The guid to synchronize the invalid breakpoint messages being sent with a delay
        private Guid _InvalidBreakpointMessageID;

        /// <summary>
        /// Adds a single breakpoint to the UI and the backup list
        /// </summary>
        /// <param name="text">The current text, if available</param>
        /// <param name="start">The start index for the breakpoint</param>
        /// <param name="offset">The vertical offset of the selected line</param>
        private void AddSingleBreakpoint([CanBeNull] String text, int start, double offset)
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
            if (BreakpointsInfo.TryGetValue(coordinate.Y, out Tuple<Ellipse, Rectangle> previous))
            {
                // Remove the previous breakpoint
                BreakpointsCanvas.Children.Remove(previous.Item1);
                BreakLinesCanvas.Children.Remove(previous.Item2);
                BreakpointsInfo.Remove(coordinate.Y);
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
                        Y = _Top + 12 + offset
                    }
                };
                BreakpointsCanvas.Children.Add(ellipse);
                ellipse.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y);

                // Line highlight
                Rectangle rect = new Rectangle
                {
                    Height = 19.9, // Approximate line height
                    Width = BreakLinesCanvas.ActualWidth,
                    Fill = XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("BreakpointLineBrush"),
                    RenderTransform = new TranslateTransform { Y = offset - 2 } // -2 to adjust the position with the cursor rectangle
                };
                BreakLinesCanvas.Children.Add(rect);
                rect.StartExpressionAnimation(EditBox.InnerScrollViewer, TranslationAxis.Y);

                // Store the info
                BreakpointsInfo.Add(coordinate.Y, Tuple.Create(ellipse, rect));
            }
        }

        // Updates the width of the breakpoint lines
        private void BreakLinesCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (FrameworkElement element in BreakLinesCanvas.Children.Cast<FrameworkElement>().ToArray())
                element.Width = e.NewSize.Width;
        }

        #endregion

        // Begins a new undo group when the user presses a keyboard key (before the text is actually changed)
        private void EditBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            EditBox.Document.BeginUndoGroup();
            if (e.Key == VirtualKey.Tab)
            {
                EditBox.Document.Selection.TypeText("\t");
                e.Handled = true;
            }
        }

        // Adjusts the vertical scaling of the indentation indicators
        private void IndentationInfoList_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            int count = ViewModel.Source.Count;
            if (count < 3) IndentationInfoList.SetVisualScale(null, 1, null);
            else
            {
                String lines = '\n'.Repeat(count - 1);
                Size size = lines.MeasureText(15);
                IndentationInfoList.SetVisualScale(null, (float) (size.Height / e.NewSize.Height), null);
            }
        }

        // Adjusts the vertical scaling of the git lines diff indicators
        private void GitDiffListView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            int count = ViewModel.DiffStatusSource.Count;
            if (count < 3) GitDiffListView.SetVisualScale(null, 1, null);
            else
            {
                String lines = '\n'.Repeat(count - 1);
                Size size = lines.MeasureText(15);
                GitDiffListView.SetVisualScale(null, (float)(size.Height / e.NewSize.Height), null);
            }
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
                ViewModel.SignalBreakpointsDeleted();
            });
            menuFlyout.ShowAt(this, offset);
        }

        #endregion
    }
}
