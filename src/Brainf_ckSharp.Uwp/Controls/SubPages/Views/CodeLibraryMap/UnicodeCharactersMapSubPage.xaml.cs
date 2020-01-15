using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views.CodeLibraryMap
{
    public sealed partial class UnicodeCharactersMapSubPage : UserControl, IConstrainedSubPage
    {
        public UnicodeCharactersMapSubPage()
        {
            this.InitializeComponent();
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
