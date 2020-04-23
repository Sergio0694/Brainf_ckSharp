using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Brainf_ckSharp.Uwp.Controls.SubPages.Shell.UserGuide;
using Brainf_ckSharp.Uwp.Messages.Navigation;
using Microsoft.Toolkit.Mvvm.Messaging;

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
        public double MaxExpandedHeight { get; } = 920;

        /// <summary>
        /// Updates the <see cref="Shared.ViewModels.Controls.SubPages.SettingsSubPageViewModel.BracketsOnNewLine"/> property
        /// </summary>
        /// <param name="sender">The <see cref="ComboBox"/> being used</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> for the selection event</param>
        private void BracketFormattingStyle_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is BracketsFormattingStyle value)
                ViewModel.BracketsOnNewLine = value == BracketsFormattingStyle.NewLine;
        }

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

        /// <summary>
        /// Updates the <see cref="Shared.ViewModels.Controls.SubPages.SettingsSubPageViewModel.StartingView"/> property
        /// </summary>
        /// <param name="sender">The <see cref="ComboBox"/> being used</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> for the selection event</param>
        private void StartingView_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is ViewType value)
                ViewModel.StartingView = value;
        }

        /// <summary>
        /// Shows the user guide and the PBrain section
        /// </summary>
        /// <param name="sender">The <see cref="HyperlinkButton"/> being clicked</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> for the click event</param>
        private void ShowPBrainButtonsInfo_Clicked(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(SubPageNavigationRequestMessage.To<UserGuideSubPage>());
        }

        /// <summary>
        /// Updates the <see cref="Shared.ViewModels.Controls.SubPages.SettingsSubPageViewModel.MemorySize"/> property
        /// </summary>
        /// <param name="sender">The <see cref="ComboBox"/> being used</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> for the selection event</param>
        private void MemorySize_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is int value)
                ViewModel.MemorySize = value;
        }
    }
}
