using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Shell
{
    public sealed partial class ReviewPromptSubPage : UserControl, IConstrainedSubPage
    {
        public ReviewPromptSubPage()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 400;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 280;
    }
}
