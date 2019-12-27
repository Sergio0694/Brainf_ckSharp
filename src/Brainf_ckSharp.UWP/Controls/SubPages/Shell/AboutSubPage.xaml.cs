using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Legere.ViewModels.SubPages.Shell;

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
            this.DataContext = new AboutSubPageViewModel();
            this.Loaded += AboutSubPage_Loaded;
        }

        /// <summary>
        /// Gets the <see cref="AboutSubPageViewModel"/> instance for the current view
        /// </summary>
        public AboutSubPageViewModel? ViewModel => this.DataContext as AboutSubPageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 400;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 560;

        // Loads the current data when the page is loaded
        private void AboutSubPage_Loaded(object sender, RoutedEventArgs e) => _ = ViewModel!.LoadDataAsync();
    }
}
