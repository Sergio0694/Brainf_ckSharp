using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.UWP.Controls.SubPages.Interfaces;
using Brainf_ckSharp.UWP.ViewModels.Controls.SubPages;

#nullable enable

namespace Brainf_ckSharp.UWP.Controls.SubPages.Shell.UserGuide
{
    /// <summary>
    /// A sub page that displays the user guide for the app
    /// </summary>
    public sealed partial class UserGuideSubPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Creates a new <see cref="UserGuideSubPage"/> instance
        /// </summary>
        public UserGuideSubPage()
        {
            this.InitializeComponent();
            this.DataContext = new UserGuideSubPageViewModel();
        }

        /// <summary>
        /// Gets the <see cref="UserGuideSubPageViewModel"/> instance for the current view
        /// </summary>
        public UserGuideSubPageViewModel? ViewModel => this.DataContext as UserGuideSubPageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 460;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 800;
    }
}
