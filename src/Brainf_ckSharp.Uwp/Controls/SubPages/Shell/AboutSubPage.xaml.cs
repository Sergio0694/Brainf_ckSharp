using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Shell
{
    /// <summary>
    /// A sub page that displays general info on the app
    /// </summary>
    public sealed partial class AboutSubPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Creates a new <see cref="AboutSubPage"/> instance
        /// </summary>
        public AboutSubPage()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 400;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 560;
    }
}
