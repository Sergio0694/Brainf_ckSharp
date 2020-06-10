using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Microsoft.Toolkit.Uwp.UI.Extensions;

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
            CodeEditBox.ContentScroller!.StartExpressionAnimation(LineBlock, Axis.Y, VisualProperty.Offset);
            CodeEditBox.ContentElement!.SizeChanged += Brainf_ckIde_SizeChanged;
        }

        /// <summary>
        /// Updates the size of the Win2D canvas when the UI is resized
        /// </summary>
        /// <param name="sender">The <see cref="Canvas"/> that hosts the text overlays</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> for <see cref="FrameworkElement.SizeChanged"/></param>
        private void Brainf_ckIde_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        /// <summary>
        /// Updates the UI when the source code displayed into the <see cref="Brainf_ckEditBox"/> instance has finished formatting
        /// </summary>
        /// <param name="sender">The <see cref="Brainf_ckEditBox"/> instance in use</param>
        /// <param name="e">The <see cref="EventArgs"/> instance for the current event</param>
        private void CodeEditBox_OnFormattingCompleted(object sender, EventArgs e)
        {
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
        }

        /// <summary>
        /// Clears all the existing breakpoints
        /// </summary>
        /// <param name="sender">The <see cref="MenuFlyoutItem"/> that was clicked</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> for the current event</param>
        private void RemoveAllBreakpointsButton_Clicked(object sender, RoutedEventArgs e)
        {
            CodeEditBox.InvalidateOverlays();
        }
    }
}
