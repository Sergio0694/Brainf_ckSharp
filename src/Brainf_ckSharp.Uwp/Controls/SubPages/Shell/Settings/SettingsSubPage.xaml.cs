using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Brainf_ckSharp.Uwp.Controls.SubPages.Shell.UserGuide;
using Brainf_ckSharp.Uwp.Messages.Navigation;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;

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
        /// Shows the user guide and the PBrain section
        /// </summary>
        private void ShowPBrainButtonsInfo_Clicked(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(SubPageNavigationRequestMessage.To<UserGuideSubPage>());
        }

        /// <summary>
        /// Tries to restart the app when the theme is changed.
        /// </summary>
        private void ThemeRestartToolTip_OnActionButtonClick(TeachingTip sender, object args)
        {
            _ = CoreApplication.RequestRestartAsync(string.Empty);
        }
    }
}
