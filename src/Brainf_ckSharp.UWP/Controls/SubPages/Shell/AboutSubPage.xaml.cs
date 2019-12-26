using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.UWP.Controls.SubPages.Interfaces;
using Legere.ViewModels.SubPages.Shell;

#nullable enable

namespace Brainf_ckSharp.UWP.Controls.SubPages.Shell
{
    public sealed partial class AboutSubPage : UserControl, IConstrainedSubPage
    {
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
        public double MaxExpandedHeight { get; } = 720;

        // Loads the current data when the page is loaded
        private void AboutSubPage_Loaded(object sender, RoutedEventArgs e) => _ = ViewModel!.LoadDataAsync();
    }
}
