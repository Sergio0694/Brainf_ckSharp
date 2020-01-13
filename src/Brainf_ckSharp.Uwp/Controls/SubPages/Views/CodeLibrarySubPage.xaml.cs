﻿using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Brainf_ckSharp.Uwp.Models.Ide;
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
        }

        /// <summary>
        /// Gets the <see cref="CodeLibrarySubPageViewModel"/> instance for the current view
        /// </summary>
        public CodeLibrarySubPageViewModel? ViewModel => this.DataContext as CodeLibrarySubPageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 460;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = double.PositiveInfinity;

        /// <summary>
        /// Opens a clicked <see cref="CodeLibraryEntry"/> model
        /// </summary>
        /// <param name="sender">The source <see cref="ListViewItem"/> instance</param>
        /// <param name="e">The <see cref="ItemClickEventArgs"/> instance with the clicked <see cref="CodeLibraryEntry"/> model</param>
        private void CodeEntriesList_ItemClicked(object sender, ItemClickEventArgs e)
        {
            _ = ViewModel!.OpenFileAsync(e.ClickedItem);
        }
    }
}
