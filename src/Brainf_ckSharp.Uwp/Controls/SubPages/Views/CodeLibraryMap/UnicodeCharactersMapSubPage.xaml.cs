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
            this.DataContext = new UnicodeCharactersMapSubPageViewModel();
        }

        /// <summary>
        /// Gets the <see cref="UnicodeCharactersMapSubPageViewModel"/> instance for the current view
        /// </summary>
        public UnicodeCharactersMapSubPageViewModel? ViewModel => this.DataContext as UnicodeCharactersMapSubPageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 520;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = double.PositiveInfinity;
    }
}
