using System;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;
using Microsoft.Toolkit.HighPerformance.Extensions;
using UICompositionAnimations.Enums;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    /// <summary>
    /// An interactive IDE for the Brainf*ck/PBrain language
    /// </summary>
    public sealed partial class Brainf_ckIde : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="Brainf_ckIde"/> instance
        /// </summary>
        public Brainf_ckIde()
        {
            this.InitializeComponent();
            this.Loaded += Brainf_ckIde_Loaded;
        }

        /// <summary>
        /// Performs some additional UI setup when the control is loaded
        /// </summary>
        /// <param name="sender">The current <see cref="Brainf_ckIde"/> instance</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance for the <see cref="FrameworkElement.Loaded"/> event</param>
        private void Brainf_ckIde_Loaded(object sender, RoutedEventArgs e)
        {
            CodeEditBox.ContentScroller!.StartExpressionAnimation(LineBlock, Axis.Y);
            CodeEditBox.ContentScroller.StartExpressionAnimation(IdeOverlaysCanvas, Axis.Y);
            CodeEditBox.ContentElement!.SizeChanged += Brainf_ckIde_SizeChanged;

            // Manually adjust the Win2D canvas size here, since when this handler runs
            // for the code editor, the first size changed event for the inner content
            // element has already been raised. Doing this fixes the Win2D canvas
            // size without the user having to first manually resize the app window.
            IdeOverlaysCanvas.Height = CodeEditBox.ContentElement.ActualHeight;
            IdeOverlaysCanvas.Width = CodeEditBox.ContentElement.ActualWidth + 72;
        }

        /// <summary>
        /// Updates the size of the Win2D canvas when the UI is resized
        /// </summary>
        /// <param name="sender">The <see cref="Canvas"/> that hosts the text overlays</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> for <see cref="FrameworkElement.SizeChanged"/></param>
        private void Brainf_ckIde_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            IdeOverlaysCanvas.Height = e.NewSize.Height + 20;
            IdeOverlaysCanvas.Width = e.NewSize.Width + 72;
        }

        /// <summary>
        /// Updates the UI when the source code displayed into the <see cref="Brainf_ckEditBox"/> instance is about to change
        /// </summary>
        /// <param name="sender">The <see cref="Brainf_ckEditBox"/> instance in use</param>
        /// <param name="args">The <see cref="RichEditBoxTextChangingEventArgs"/> instance for the current event</param>
        private void CodeEditBox_OnTextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            // This needs to be invoked before the text changes, to avoid
            // synchronization issues leading to crashes in the text control
            UpdateBreakpointsInfo();
        }

        /// <summary>
        /// Updates the UI when the source code displayed into the <see cref="Brainf_ckEditBox"/> instance changes
        /// </summary>
        /// <param name="sender">The <see cref="Brainf_ckEditBox"/> instance in use</param>
        /// <param name="args">The arguments for the new Brainf*ck/Pbrain source code being displayed</param>
        private void CodeEditBox_TextChanged(Brainf_ckEditBox sender, TextChangedEventArgs args)
        {
            TextChanged?.Invoke(this, args);

            int numberOfLines = args.PlainText.Count(Characters.CarriageReturn);

            UpdateLineIndicators(numberOfLines);
            UpdateDiffInfo(args.PlainText);
            UpdateIndentationInfo(args.PlainText, args.ValidationResult.IsSuccessOrEmptyScript, numberOfLines);

            IdeOverlaysCanvas.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="CursorPositionChanged"/> event
        /// </summary>
        /// <param name="sender">The <see cref="Brainf_ckEditBox"/> instance in use</param>
        /// <param name="args">The arguments for the cursor movement</param>
        private void CodeEditBox_CursorPositionChanged(Brainf_ckEditBox sender, CursorPositionChangedEventArgs args)
        {
            CursorPositionChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Updates the <see cref="TextBlock"/> that displays the line number next to each line
        /// </summary>
        /// <param name="numberOfLines">The current number of lines being displayed</param>
        private void UpdateLineIndicators(int numberOfLines)
        {
            LineBlock.Text = TextGenerator.GetLineNumbersText(numberOfLines);
        }

        /// <summary>
        /// Sets up a breakpoint when the user taps on the breakpoints area
        /// </summary>
        /// <param name="sender">The <see cref="Canvas"/> instance in use</param>
        /// <param name="e">The <see cref="TappedRoutedEventArgs"/> instance for the event</param>
        private void BreakpointsCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Calculate the target vertical offset for the tap
            double yOffset =
                e.GetPosition((Border)sender).Y - 8 -         // Tap Y offset and adjustment
                CodeEditBox.VerticalScrollBarMargin.Top +     // Top internal padding
                CodeEditBox.ContentScroller!.VerticalOffset;  // Vertical scroll offset

            // Get the range aligned to the left edge of the tapped line
            ITextRange range = CodeEditBox.Document.GetRangeFromPoint(new Point(0, yOffset), PointOptions.ClientCoordinates);
            range.GetRect(PointOptions.Transform, out Rect line, out _);

            // Get the line number
            int lineNumber = CodeEditBox.Text.AsSpan(0, range.StartPosition).Count(Characters.CarriageReturn) + 1;

            if (lineNumber == 1) return;

            // Store or remove the breakpoint
            if (BreakpointIndicators.ContainsKey(lineNumber))
            {
                BreakpointIndicators.Remove(lineNumber);

                if (BreakpointIndicators.Count == 0) BreakpointsBorder.ContextFlyout = null;

                BreakpointRemoved?.Invoke(this, lineNumber);
            }
            else
            {
                if (BreakpointIndicators.Count == 0) BreakpointsBorder.ContextFlyout = BreakpointsMenuFlyout;

                BreakpointIndicators.GetOrAddValueRef(lineNumber) = (float)line.Top;

                BreakpointAdded?.Invoke(this, lineNumber);
            }

            UpdateBreakpointsInfo();

            IdeOverlaysCanvas.Invalidate();
            CodeEditBox.InvalidateOverlays();
        }

        /// <summary>
        /// Clears all the existing breakpoints
        /// </summary>
        /// <param name="sender">The <see cref="MenuFlyoutItem"/> that was clicked</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> for the current event</param>
        private void RemoveAllBreakpointsButton_Clicked(object sender, RoutedEventArgs e)
        {
            BreakpointIndicators.Clear();

            IdeOverlaysCanvas.Invalidate();
        }
    }
}
