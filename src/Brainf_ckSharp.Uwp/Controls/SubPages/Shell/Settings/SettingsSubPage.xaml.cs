using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Shell.Settings
{
    /// <summary>
    /// A sub page that displays the available app settings
    /// </summary>
    public sealed partial class SettingsSubPage : UserControl, IConstrainedSubPage
    {
        public SettingsSubPage()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 520;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = double.PositiveInfinity;

        /// <summary>
        /// Updates the <see cref="Shared.ViewModels.Controls.SubPages.SettingsSubPageViewModel.TabLength"/> property
        /// </summary>
        /// <param name="sender">The <see cref="ComboBox"/> being used</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> for the selection event</param>
        private void TabLength_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is int value)
                ViewModel.TabLength = value;
        }
    }
}
