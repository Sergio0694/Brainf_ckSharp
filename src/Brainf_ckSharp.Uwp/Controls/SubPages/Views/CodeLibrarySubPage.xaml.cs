using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views
{
    /// <summary>
    /// A sub page that displays the library of recent and sample source codes
    /// </summary>
    public sealed partial class CodeLibrarySubPage : UserControl, IConstrainedSubPage
    {
        public CodeLibrarySubPage()
        {
            this.InitializeComponent();
            this.DataContext = new CodeLibrarySubPageViewModel();
            
            _ = ViewModel.LoadAsync();
        }

        /// <summary>
        /// Gets the <see cref="CodeLibrarySubPageViewModel"/> instance for the current view
        /// </summary>
        public CodeLibrarySubPageViewModel? ViewModel => this.DataContext as CodeLibrarySubPageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 460;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = double.PositiveInfinity;
    }
}
