using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;
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
        }

        /// <summary>
        /// Updates the size of the Win2D canvas when the UI is resized
        /// </summary>
        /// <param name="sender">The <see cref="Canvas"/> that hosts the text overlays</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> for <see cref="FrameworkElement.SizeChanged"/></param>
        private void Brainf_ckIde_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            IdeOverlaysCanvas.Height = e.NewSize.Height;
            IdeOverlaysCanvas.Width = e.NewSize.Width + 72;
        }

        /// <summary>
        /// Updates the UI when the source code displayed into the <see cref="Brainf_ckEditBox"/> instance changes
        /// </summary>
        /// <param name="sender">The <see cref="Brainf_ckEditBox"/> instance in use</param>
        /// <param name="args">The new Brainf*ck/Pbrain source code being displayed</param>
        private void CodeEditBox_OnPlainTextChanged(Brainf_ckEditBox sender, string args)
        {
            int numberOfLines = args.Count('\r');

            UpdateLineIndicators(args, numberOfLines);
            UpdateIndentationInfo(args, numberOfLines);
        }

        /// <summary>
        /// Updates the <see cref="TextBlock"/> that displays the line number next to each line
        /// </summary>
        /// <param name="text">The new text being diplayed in the IDE</param>
        /// <param name="numberOfLines">The current number of lines being displayed</param>
        private void UpdateLineIndicators(string text, int numberOfLines)
        {
            LineBlock.Text = TextGenerator.GetLineNumbersText(numberOfLines);
        }
    }
}
