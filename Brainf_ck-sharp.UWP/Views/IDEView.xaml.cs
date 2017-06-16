﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.FlyoutService;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
using Brainf_ck_sharp_UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.Views
{
    public sealed partial class IDEView : UserControl
    {
        public IDEView()
        {
            Loaded += IDEView_Loaded;
            this.InitializeComponent();
            DataContext = new IDEViewModel(EditBox.Document);
            ViewModel.PlayRequested += ViewModel_PlayRequested;
            EditBox.Document.GetText(TextGetOptions.None, out String text);
            _PreviousText = text;
            Messenger.Default.Register<SourceCodeLoadingRequestedMessage>(this, m =>
            {
                _LoadedCode = m.RequestedCode;
                LoadCode(m.RequestedCode.Code, true);
            });
            Messenger.Default.Register<SaveSourceCodeRequestMessage>(this, m => ManageSaveCodeRequest(m.RequestType));
        }

        private async void ManageSaveCodeRequest(CodeSaveType type)
        {
            EditBox.Document.GetText(TextGetOptions.None, out String text);
            SaveCodePromptFlyout flyout = new SaveCodePromptFlyout(text);
            var result = await FlyoutManager.Instance.ShowAsync("Save code", flyout, new Thickness(12, 12, 16, 12), FlyoutDisplayMode.ActualHeight);
            if (result == FlyoutResult.Confirmed)
            {
                await SQLiteManager.Instance.SaveCodeAsync(flyout.Title, text);
                NotificationsManager.ShowNotification(0xEC24.ToSegoeMDL2Icon(), "Code saved", "The new source code has been saved correctly",
                    NotificationType.Default);
            }
        }

        // Gets the loaded code the user is currently working on, if present
        private SourceCode _LoadedCode;

        private void ViewModel_PlayRequested(object sender, string e)
        {
            EditBox.Document.GetText(TextGetOptions.None, out String text);
            InterpreterExecutionSession session = Brainf_ckInterpreter.InitializeSession(new[] { text }, e, 64, 1000);
            IDERunResultFlyout flyout = new IDERunResultFlyout(session);
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("RunTitle"), flyout, new Thickness()).Forget();
            flyout.ViewModel.LoadGroupsAsync().Forget();
        }

        // Initializes the scroll events for the code
        private void IDEView_Loaded(object sender, RoutedEventArgs e)
        {
            CursorAnimation.Begin();
            ScrollViewer scroller = EditBox.FindChild<ScrollViewer>();
            scroller.ViewChanged += Scroller_ViewChanged;
        }

        // Updates the position of the line numbers when the edit box is scrolled
        private void Scroller_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // Keep the line numbers and the current cursor in sync with the code
            float target = (float)(_Top - 12 - EditBox.VerticalScrollViewerOffset);
            LinesGrid.SetVisualOffsetAsync(TranslationAxis.Y, target);
            IndentationInfoList.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 10 - EditBox.VerticalScrollViewerOffset));
            GitDiffListView.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 10 - EditBox.VerticalScrollViewerOffset));
            Point selectionOffset = EditBox.ActualSelectionVerticalOffset;
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + selectionOffset.Y));
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + selectionOffset.Y));
            CursorTransform.X = -EditBox.HorizontalScrollViewerOffset;
            GuidesTransform.Y = -EditBox.VerticalScrollViewerOffset;
            GuidesTransform.X = -EditBox.HorizontalScrollViewerOffset;
        }

        public IDEViewModel ViewModel => DataContext.To<IDEViewModel>();

        // The current top margin
        private double _Top;

        /// <summary>
        /// Adjusts the top margin of the content in the list
        /// </summary>
        /// <param name="height">The desired height</param>
        public void AdjustTopMargin(double height)
        {
            _Top = height;
            LinesGrid.SetVisualOffsetAsync(TranslationAxis.Y, (float)(height - 12)); // Adjust the initial offset of the line numbers and indicators
            IndentationInfoList.SetVisualOffsetAsync(TranslationAxis.Y, (float)(height + 10));
            GitDiffListView.SetVisualOffsetAsync(TranslationAxis.Y, (float)(height + 10));
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(height + 8));
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.Y, (float) (height + 8));
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.X, 4);
            BracketsParentGrid.SetVisualOffsetAsync(TranslationAxis.Y, (float)height);
            EditBox.Padding = new Thickness(4, _Top + 8, 4, 8);
            EditBox.ScrollBarMargin = new Thickness(0, _Top, 0, 0);
        }

        // Updates the line numbers displayed next to the code box
        private void EditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            DrawLineNumbers();
            DrawBracketGuides(null);
            ViewModel.UpdateIndentationInfo(_Brackets);
        }

        #region UI overlays

        /// <summary>
        /// Updates the line numbers shown next to the code edit box
        /// </summary>
        private void DrawLineNumbers()
        {
            // Draw the line numbers in the TextBlock next to the code
            int count = EditBox.GetLinesCount();
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < count; i++)
            {
                builder.Append($"\n{i}");
            }
            LineBlock.Text = builder.ToString();
        }

        // The backup of the indexes of the brackets in the text
        private IReadOnlyList<(int, int, char)> _Brackets;

        /// <summary>
        /// Redraws the column guides if necessary
        /// </summary>
        /// <param name="code">The current text, if already available</param>
        private void DrawBracketGuides(String code)
        {
            // Get the text, clear the current guides and make sure the syntax is currently valid
            if (code == null) EditBox.Document.GetText(TextGetOptions.None, out code);

            // Check the current syntax
            (bool valid, _) = Brainf_ckInterpreter.CheckSourceSyntax(code);
            if (!valid)
            {
                BracketGuidesCanvas.Children.Clear();
                _Brackets = null;
                return;
            }

            // Build the indexes for the current state
            List<(int, int, char)> indexes = new List<(int, int, char)>();
            foreach ((char c, int i) in code.Select((c, i) => (c, i)))
            {
                if (c == '[' || c == ']')
                {
                    (int x, int y) = code.FindCoordinates(i);
                    indexes.Add((x, y, c));
                }
            }

            // Check if the brackets haven't been changed or moved
            if (_Brackets != null && 
                _Brackets.Count == indexes.Count &&
                _Brackets.Zip(indexes, (first, second) => first.Equals(second)).All(b => b))
            {
                return;
            }
            _Brackets = indexes;
            BracketGuidesCanvas.Children.Clear();

            // Draw the guides for each brackets pair
            foreach ((char c, int i) in code.Select((c, i) => (c, i)))
            {
                // Get the index of the corresponding closing bracket (only if they're not on the same line)
                if (c != '[') continue;
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
                if (target == -1) continue;

                // Get the initial and ending range
                ITextRange range = EditBox.Document.GetRange(i, i);
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
                    Stroke = new SolidColorBrush(Colors.LightGray),
                    StrokeDashArray = new DoubleCollection { 4 },
                    Y1 = 0,
                    Y2 = top
                };
                guide.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 30 + open.Top));
                guide.SetVisualOffsetAsync(TranslationAxis.X, (float)(open.X + 6));
                BracketGuidesCanvas.Children.Add(guide);
            }
        }

        /// <summary>
        /// Gets the approximate height of each line of code in the IDE
        /// </summary>
        public double LineApproximateHeight // TODO: approximate this when the lines count changes and bind it to the items height of the ListView
        {
            get => (double)GetValue(LineApproximateHeightProperty);
            set => SetValue(LineApproximateHeightProperty, value);
        }

        public static readonly DependencyProperty LineApproximateHeightProperty = DependencyProperty.Register(
            nameof(LineApproximateHeight), typeof(double), typeof(IDEView), new PropertyMetadata(19.94998046875));

        #endregion

        // Gets the backup of the text in the IDE
        private String _PreviousText;

        private void EditBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            /* ====================
             * Syntax highlight
             * ================= */

            // Unsubscribe from the text events and batch the updates
            EditBox.SelectionChanged -= EditBox_OnSelectionChanged;
            EditBox.TextChanged -= EditBox_OnTextChanged;
            EditBox.Document.BatchDisplayUpdates();

            // Get the current text and backup the current index
            EditBox.Document.GetText(TextGetOptions.None, out String text);
            int start = EditBox.Document.Selection.StartPosition;

            // Single character entered
            bool refreshBrackets = false;
            if (text.Length == _PreviousText.Length + 1)
            {
                // Get the last character and apply the right color
                ITextRange range = EditBox.Document.GetRange(start - 1, start);
                range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.GetSyntaxHighlightColorFromChar(range.Character);

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
                        // Edge case: the user was already on an empty and indented line when opening the bracket
                        bool edge = false;
                        int lastCr = trailer.LastIndexOf('\r');
                        if (lastCr != -1)
                        {
                            // Autocomplete without the first blank line
                            String lastLine = trailer.Substring(lastCr, trailer.Length - lastCr);
                            if (lastLine.Skip(1).All(c => c == '\t') && lastLine.Length == indents + 1)
                            {
                                EditBox.Document.Selection.TypeText($"\r{tabs}\t\r{tabs}]");
                                edge = true;
                            }
                        }

                        // Default autocomplete: new line and [ ] brackets
                        if (!edge)
                        {
                            range.Delete(TextRangeUnit.Character, 1);
                            EditBox.Document.Selection.TypeText($"\r{tabs}[\r{tabs}\t\r{tabs}]");
                        }
                        
                        // Apply the right color and move the selection at the center of the brackets
                        ITextRange bracketsRange = EditBox.Document.GetRange(start, EditBox.Document.Selection.EndPosition);
                        bracketsRange.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.GetSyntaxHighlightColorFromChar('[');
                        EditBox.Document.Selection.Move(TextRangeUnit.Character, -(indents + 2));
                        DrawLineNumbers();
                        refreshBrackets = true;
                    }
                    else if (range.Character == '\r')
                    {
                        // New line, tabs needed
                        if (tabs.Length > 0) EditBox.Document.Selection.TypeText(tabs);
                        DrawLineNumbers();
                        refreshBrackets = true;
                    }
                }
            }

            // Display the text updates
            int batch;
            do
            {
                batch = EditBox.Document.ApplyDisplayUpdates();
            } while (batch != 0);

            // Get the updated text
            EditBox.Document.GetText(TextGetOptions.None, out text);
            ViewModel.UpdateGitDiffStatus(_LoadedCode?.Code ?? _PreviousText, text);
            _PreviousText = text;

            // Update the bracket guides
            if (refreshBrackets)
            {
                DrawBracketGuides(text);
                ViewModel.UpdateIndentationInfo(_Brackets);
            }

            // Move the cursor to the right position
            Point selectionOffset = EditBox.ActualSelectionVerticalOffset;
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + selectionOffset.Y));
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.X, (float)(selectionOffset.X + 4));

            // Update the visibility and the position of the cursor
            CursorBorder.SetVisualOpacity(EditBox.Document.Selection.Length.Abs() > 0 ? 0 : 1);
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + EditBox.ActualSelectionVerticalOffset.Y));

            // Notify the UI
            ViewModel.SendMessages(text);

            // Restore the event handlers
            EditBox.Document.ApplyDisplayUpdates();
            EditBox.SelectionChanged += EditBox_OnSelectionChanged;
            EditBox.TextChanged += EditBox_OnTextChanged;

            // Scroll if needed
            EditBox.TryScrollToSelection();
        }

        private void EditBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            CursorAnimation.Stop();
            CursorRectangle.Visibility = Visibility.Collapsed;
            VisualStateManager.GoToState(this, "Focused", false);
        }

        private void EditBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            CursorRectangle.Visibility = Visibility.Visible;
            CursorAnimation.Begin();
            VisualStateManager.GoToState(this, "Default", false);
        }

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
        private void LoadCode(String code, bool overwrite)
        {
            // Disable the handlers
            EditBox.SelectionChanged -= EditBox_OnSelectionChanged;
            EditBox.TextChanged -= EditBox_OnTextChanged;
            EditBox.Document.BatchDisplayUpdates();

            // Paste and highlight the text
            if (overwrite) EditBox.Document.Selection.SetRange(0, int.MaxValue);
            EditBox.Document.Selection.SetText(TextSetOptions.None, code);
            int start = EditBox.Document.Selection.StartPosition, end = EditBox.Document.Selection.EndPosition;
            for (int i = start; i < end - 1; i++)
            {
                ITextRange range = EditBox.Document.GetRange(i, i + 1);
                range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.GetSyntaxHighlightColorFromChar(range.Character);
            }
            if (overwrite) EditBox.Document.Selection.SetRange(0, 0);
            else EditBox.Document.Selection.StartPosition = end;

            // Refresh the UI
            EditBox.Document.ApplyDisplayUpdates();
            EditBox.Document.GetText(TextGetOptions.None, out code);
            _PreviousText = code;
            DrawLineNumbers();
            DrawBracketGuides(code);
            ViewModel.UpdateIndentationInfo(_Brackets);
            ViewModel.UpdateGitDiffStatus(_LoadedCode?.Code ?? _PreviousText, code);
            ViewModel.SendMessages(code);

            // Restore the handlers
            EditBox.SelectionChanged += EditBox_OnSelectionChanged;
            EditBox.TextChanged += EditBox_OnTextChanged;
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
        }
    }
}