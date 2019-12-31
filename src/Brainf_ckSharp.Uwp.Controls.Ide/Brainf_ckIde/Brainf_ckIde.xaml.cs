using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        }
    }
}
