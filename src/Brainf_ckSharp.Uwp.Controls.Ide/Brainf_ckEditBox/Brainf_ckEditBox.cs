using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.System;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.Windows.System;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.Windows.UI.Text;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    /// <summary>
    /// A custom <see cref="RichEditBox"/> that formats Brainf*ck/PBrain code
    /// </summary>
    public sealed partial class Brainf_ckEditBox : RichEditBox
    {
        /// <summary>
        /// Creates a new <see cref="Brainf_ckEditBox"/> instance
        /// </summary>
        public Brainf_ckEditBox()
        {
            // The data context is set to self to enable bindings from the context
            // menu buttons. This is a workaround for the fact that template bindings
            // can't be used there, as the flyout is not part of the control template.
            // Having self as the data context allows standard bindings to work instead.
            DataContext = this;

            // Set the tab length to align with Visual Studio
            Document.SetTabLength(8);

            SelectionChanging += Brainf_ckEditBox_SelectionChanging;
            SelectionChanged += Brainf_ckEditBox_SelectionChanged;
            TextChanging += MarkdownRichEditBox_TextChanging;
            base.TextChanged += MarkdownRichEditBox_TextChanged;
            Paste += Brainf_ckEditBox_Paste;
            Loaded += (s, e) => EnableClipboardMonitoring(true);
            Unloaded += (s, e) => EnableClipboardMonitoring(false);
        }

        /// <summary>
        /// Updates <see cref="_SelectionLength"/> with the length of the current selection
        /// </summary>
        /// <param name="sender">The current <see cref="Brainf_ckEditBox"/> instance</param>
        /// <param name="args">The <see cref="RichEditBoxSelectionChangingEventArgs"/> instance for the <see cref="RichEditBox.SelectionChanging"/> event</param>
        private void Brainf_ckEditBox_SelectionChanging(RichEditBox sender, RichEditBoxSelectionChangingEventArgs args)
        {
            _SelectionStart = args.SelectionStart;
            _SelectionLength = args.SelectionLength;

            IsTextSelected = args.SelectionLength > 0;
        }

        /// <summary>
        /// Adjusts the UI when the selection changes
        /// </summary>
        /// <param name="sender">The current <see cref="Brainf_ckEditBox"/> instance</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance for the <see cref="RichEditBox.SelectionChanged"/> event</param>
        private void Brainf_ckEditBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ScrollToSelection(out Rect rect);

            // Adjust the UI of the selected line highlight and the cursor indicator.
            // Both elements are translated to the right position and made visible
            // if the current selection is not collapsed to a single point, otherwise
            // they're both hidden. This is the same behavior of Visual Studio.
            if (_SelectionLength > 0)
            {
                _SelectionHighlightBorder!.Opacity = 0;
                _CursorIndicatorRectangle!.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Line highlight
                _SelectionHighlightBorder!.Opacity = 1;
                ((TranslateTransform)_SelectionHighlightBorder.RenderTransform).Y = rect.Top + Padding.Top;

                // Cursor indicator
                _CursorIndicatorRectangle!.Visibility = Visibility.Visible;
                TranslateTransform cursorTransform = (TranslateTransform)_CursorIndicatorRectangle.RenderTransform;
                cursorTransform.X = rect.X + Padding.Left;
                cursorTransform.Y = rect.Y + Padding.Top;
            }

            var position = Text.CalculateCoordinates(Document.Selection.EndPosition);
            var args = new CursorPositionChangedEventArgs(position.Row + 1, position.Column + 1);

            // Signal the cursor movement
            CursorPositionChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Tries to scroll to the current selection
        /// </summary>
        /// <param name="rect">The resulting selection coordinates</param>
        private void ScrollToSelection(out Rect rect)
        {
            Document.Selection.GetRect(PointOptions.Transform, out rect, out _);

            double
                verticalOffset = ContentScroller!.VerticalOffset,
                horizontalOffset = ContentScroller.HorizontalOffset,
                viewportHeight = ContentScroller.ViewportHeight - VerticalScrollBarMargin.Top,
                viewportWidth = ContentScroller.ViewportWidth,
                transformedVerticalOffset = rect.Top - verticalOffset;

            const double NegativeLeftOffsetBeforeSelection = 12;
            const double HorizontalScrollingThreshold = 20;
            const double RightOffsetAfterSelection = 32;

            // Calculate the target horizontal offset
            double? horizontal;
            if (rect.Left < horizontalOffset) horizontal = rect.Left - NegativeLeftOffsetBeforeSelection;
            else if (rect.Left > viewportWidth + horizontalOffset - HorizontalScrollingThreshold)
            {
                horizontal = rect.Left - viewportWidth + RightOffsetAfterSelection;
            }
            else horizontal = null;

            const double VerticalScrollingThreshold = 32;
            const double BottomOffsetBelowSelection = 32;

            // Calculate the target vertical offset
            double? vertical;
            if (transformedVerticalOffset < 0) vertical = verticalOffset + transformedVerticalOffset;
            else if (transformedVerticalOffset > viewportHeight - VerticalScrollingThreshold)
            {
                vertical = verticalOffset + (transformedVerticalOffset - viewportHeight) + BottomOffsetBelowSelection;
            }
            else vertical = null;

            // Scroll to selection
            ContentScroller.ChangeView(horizontal, vertical, null, false);
        }

        /// <summary>
        /// Shows the syntax error tooltip, if an error is present
        /// </summary>
        public async void TryShowSyntaxErrorToolTip()
        {
            if (_SyntaxValidationResult.IsSuccessOrEmptyScript) return;

            int errorPosition = _SyntaxValidationResult.ErrorOffset;

            TaskCompletionSource<object?> tcs = new TaskCompletionSource<object?>();

            bool hasViewChanged = false;

            // Register the events to track the change view request
            ContentScroller!.ViewChanged += NotifyViewChanged;
            ContentScroller.ViewChanging += NotifyViewChanging;

            // Sets the task to monitor the view change
            void NotifyViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
            {
                tcs.TrySetResult(null);
            }

            // Sets the tracking for the view changing
            void NotifyViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
            {
                hasViewChanged = true;
            }

            // Set the selection to the error position, otherwise just scroll there
            if (Document.Selection.StartPosition == errorPosition &&
                Document.Selection.Length == 0)
            {
                ScrollToSelection(out _);
            }
            else Document.Selection.SetRange(errorPosition, errorPosition);

            // Wait a minimum delay
            await Task.Delay(100);

            if (hasViewChanged)
            {
                // If we are here, it means the view has at least started to change.
                // This means that we can safely wait for the view change to be completed.
                // This is necessary to avoid a situation where the scroller was already
                // in the right spot, and the view changed event wouldn't have ever been raised.
                await tcs.Task;
                await Task.Delay(250); // This is needed to ensure the tooltip aligns properly
            }

            // Remove the one-shot handler
            ContentScroller!.ViewChanged -= NotifyViewChanged;
            ContentScroller.ViewChanging -= NotifyViewChanging;

            // Disable the scrolling while the tooltip is opened
            ContentScroller!.IsHitTestVisible = false;

            // Reset the target to ensure the right target coordinates are used
            _SyntaxErrorToolTip!.IsOpen = false;
            _SyntaxErrorToolTip.Target = null;
            _SyntaxErrorToolTip.Target = _CursorIndicatorRectangle!;
            _SyntaxErrorToolTip!.IsOpen = true;
        }

        /// <summary>
        /// Retrieves, inserts and formats text being pasted by the user
        /// </summary>
        /// <param name="sender">The current <see cref="Brainf_ckEditBox"/> instance</param>
        /// <param name="e">The <see cref="TextControlPasteEventArgs"/> instance for the <see cref="RichEditBox.Paste"/> event</param>
        private async void Brainf_ckEditBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            e.Handled = true;

            // Insert the text if there is some available
            if (await ClipboardHelper.TryGetTextAsync() is string text)
            {
                InsertText(text);
            }
        }

        // Checks when the text changes and applies the syntax highlight
        private void MarkdownRichEditBox_TextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            if (!args.IsContentChanging) return;

            ApplySyntaxHighlight();
        }

        private bool _IsUndoGroupingEnabled;

        /// <summary>
        /// Gets or sets whether or not undo grouping should be enabled
        /// </summary>
        private bool IsUndoGroupingEnabled
        {
            set
            {
                if (_IsUndoGroupingEnabled != value)
                {
                    _IsUndoGroupingEnabled = value;
                    if (value) Document.BeginUndoGroup();
                    else Document.EndUndoGroup();
                }
            }
        }

        // Ends the undo grouping when the text is finally changed
        private void MarkdownRichEditBox_TextChanged(object sender, RoutedEventArgs e) => IsUndoGroupingEnabled = false;

        /// <summary>
        /// The list of shortcut keys to ignore
        /// </summary>
        private readonly HashSet<VirtualKey> IgnoredShortcuts = new HashSet<VirtualKey>(new[]
        {
            VirtualKey.E, VirtualKey.R, // Indent right
            VirtualKey.J,               // Can't remember now
            VirtualKey.L,               // Reset indent
            VirtualKey.B,               // Bold
            VirtualKey.I,               // Italic, or TAB
            VirtualKey.U                // Underline
        });

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            // Disable the unwanted shortcuts
            bool ctrl = VirtualKey.Control.IsDown();
            if (ctrl && IgnoredShortcuts.Contains(e.Key))
            {
                goto HandleAndReturn;
            }

            IsUndoGroupingEnabled = true;

            // Undo request with CTRL + Z
            if (ctrl && e.Key == VirtualKey.Z && Document.CanUndo())
            {
                goto BaseOnKeyDown;
            }

            // Delete/canc keys
            if (e.Key == VirtualKey.Back ||
                e.Key == VirtualKey.Delete)
            {
                _IsDeleteRequested = true;

                goto BaseOnKeyDown;
            }

            // Tab shortcuts
            if (e.Key == VirtualKey.Tab)
            {
                // There are three possible operations to perform when the
                // tab key is pressed. If the current selection is up to a
                // single character, a '\t' is typed. Otherwise, the current
                // selection is shifted forwards or backwards by adding and
                // removing tabs depending on whether the shift key is down.
                if (Math.Abs(Document.Selection.Length) <= 1)
                {
                    Document.Selection.TypeText("\t");
                }
                else if (VirtualKey.Shift.IsDown()) ShiftBackward();
                else ShiftForward();

                goto HandleAndReturn;
            }

            // Manually handle the enter key to avoid \v
            if (e.Key == VirtualKey.Enter)
            {
                Document.Selection.TypeText("\r");

                goto HandleAndReturn;
            }

            BaseOnKeyDown:
            base.OnKeyDown(e);
            return;

            HandleAndReturn:
            e.Handled = true;
        }
    }
}
