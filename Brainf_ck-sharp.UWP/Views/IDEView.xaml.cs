using System;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
using Brainf_ck_sharp_UWP.ViewModels;
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
        }

        private void ViewModel_PlayRequested(object sender, string e)
        {
            EditBox.Document.GetText(TextGetOptions.None, out String text);
            InterpreterExecutionSession session = Brainf_ckInterpreter.InitializeSession(new[] { text }, e);
            IDERunResultFlyout flyout = new IDERunResultFlyout(session);
            FlyoutManager.Instance.Show("Run", flyout, new Thickness());
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
            float target = (float) (_Top - 12 - EditBox.VerticalScrollViewerOffset);
            LinesGrid.SetVisualOffsetAsync(TranslationAxis.Y, target);
            Point selectionOffset = EditBox.ActualSelectionVerticalOffset;
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + selectionOffset.Y));
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + selectionOffset.Y));
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
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(height + 8));
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.Y, (float) (height + 8));
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.X, 4);
            TopMarginGrid.Height = height;
        }

        // Updates the line numbers displayed next to the code box
        private void EditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            DrawLineNumbers();
        }

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

        private String _PreviousText;

        private void EditBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            // Update the visibility and the position of the cursor
            CursorBorder.SetVisualOpacity(EditBox.Document.Selection.Length.Abs() > 0 ? 0 : 1);
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + EditBox.ActualSelectionVerticalOffset.Y));

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
                    }
                    else if (range.Character == '\r')
                    {
                        // New line, tabs needed
                        if (tabs.Length > 0) EditBox.Document.Selection.TypeText(tabs);
                        DrawLineNumbers();
                    }
                }
            }
            else
            {
                
            }

            // Display the text updates
            int batch;
            do
            {
                batch = EditBox.Document.ApplyDisplayUpdates();
            } while (batch != 0);

            // Get the updated text
            EditBox.Document.GetText(TextGetOptions.None, out text);
            _PreviousText = text;

            // Move the cursor to the right position
            Point selectionOffset = EditBox.ActualSelectionVerticalOffset;
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + selectionOffset.Y));
            CursorRectangle.SetVisualOffsetAsync(TranslationAxis.X, (float)(selectionOffset.X + 4));

            // Notify the UI
            ViewModel.SendMessages(text);

            // Restore the event handlers
            EditBox.Document.ApplyDisplayUpdates();
            EditBox.SelectionChanged += EditBox_OnSelectionChanged;
            EditBox.TextChanged += EditBox_OnTextChanged;
        }

        private void EditBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            CursorAnimation.Stop();
            CursorRectangle.Visibility = Visibility.Collapsed;
        }

        private void EditBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            CursorRectangle.Visibility = Visibility.Visible;
            CursorAnimation.Begin();
        }
    }
}
