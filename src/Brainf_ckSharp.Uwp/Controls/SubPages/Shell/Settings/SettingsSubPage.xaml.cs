using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Brainf_ckSharp.Uwp.Controls.SubPages.Shell.UserGuide;
using Microsoft.Extensions.DependencyInjection;

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
            this.DataContext = App.Current.Services.GetRequiredService<SettingsSubPageViewModel>();
        }

        /// <summary>
        /// Gets the <see cref="SettingsSubPageViewModel"/> instance currently in use
        /// </summary>
        public SettingsSubPageViewModel ViewModel => (SettingsSubPageViewModel)DataContext;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 520;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 808;

        /// <summary>
        /// Shows the user guide and the PBrain section
        /// </summary>
        private void ShowPBrainButtonsInfo_Clicked(object sender, RoutedEventArgs e)
        {
            App.Current.SubPageHost.DisplaySubFramePage(new UserGuideSubPage());
        }
    }
}
