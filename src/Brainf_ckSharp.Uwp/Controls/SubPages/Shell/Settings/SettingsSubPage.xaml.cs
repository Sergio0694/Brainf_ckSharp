using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        public double MaxExpandedHeight { get; } = 808;

        /// <summary>
        /// Shows the user guide and the PBrain section
        /// </summary>
        private void ShowPBrainButtonsInfo_Clicked(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(SubPageNavigationRequestMessage.To<UserGuideSubPage>());
        }
    }
}
