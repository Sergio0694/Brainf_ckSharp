using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;

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
            SelectionChanging += Brainf_ckEditBox_SelectionChanging;
            SelectionChanged += Brainf_ckEditBox_SelectionChanged;
            TextChanging += MarkdownRichEditBox_TextChanging;
            TextChanged += MarkdownRichEditBox_TextChanged;
            Paste += Brainf_ckEditBox_Paste;
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
        }

        /// <summary>
        /// Adjusts the scrolling offset on both axes when the selection changes
        /// </summary>
        /// <param name="sender">The current <see cref="Brainf_ckEditBox"/> instance</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance for the <see cref="RichEditBox.SelectionChanged"/> event</param>
        private void Brainf_ckEditBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Document.Selection.GetRect(PointOptions.Transform, out Rect rect, out _);

            double
                verticalOffset = _ContentScroller.VerticalOffset,
                horizontalOffset = _ContentScroller.HorizontalOffset,
                viewportHeight = _ContentScroller.ViewportHeight - VerticalScrollBarMargin.Top,
                viewportWidth = _ContentScroller.ViewportWidth,
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

            _ContentScroller.ChangeView(horizontal, vertical, null, false);
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
            if (await ClipboardHelper.TryGetTextAsync() is { } text)
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
                /* There are three possible operations to perform when the
                 * tab key is pressed. If the current selection is up to a
                 * single character, a '\t' is typed. Otherwise, the current
                 * selection is shifted forwards or backwards by adding and
                 * removing tabs depending on whether the shift key is down. */
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
