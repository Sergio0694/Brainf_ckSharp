using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.UserControls.Flyouts.UserGuide;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo
{
    public sealed partial class DevInfoFlyout : UserControl
    {
        public DevInfoFlyout()
        {
            this.InitializeComponent();
            PackageVersion version = SystemInformation.ApplicationVersion;
            BuildBlock.Text = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        // Opens the Store review page for the app
        private void RateStoreButton_Click(object sender, RoutedEventArgs e)
        {
            LauncherHelper.OpenStoreAppReviewPageAsync().AsTask().Forget();
            if (AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.ReviewPromptShown), out bool reviewed) && !reviewed)
            {
                AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.ReviewPromptShown), true, SettingSaveMode.OverwriteIfExisting);
            }
        }

        // Sends a feedback email
        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            EmailHelper.SendFeedbackEmail().Forget();
        }

        // Opens the Twitter page
        private void TwitterButton_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("https://twitter.com/sergiopedri")).AsTask().Forget();
        }

        // Shows the changelog flyout
        private void ShowChangelogButton_Click(object sender, RoutedEventArgs e)
        {
            ChangelogViewFlyout flyout = new ChangelogViewFlyout();
            Task.Delay(100).ContinueWith(t => flyout.ViewModel.LoadGroupsAsync(), TaskScheduler.FromCurrentSynchronizationContext()).Forget();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("Changelog"), flyout, null, new Thickness(),
                FlyoutDisplayMode.ScrollableContent, true).Forget();
        }

        // Show the user guide
        private void UserGuideButton_Click(object sender, RoutedEventArgs e)
        {
            UserGuideViewerControl guide = new UserGuideViewerControl();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("UserGuide"), guide, null, new Thickness()).Forget();
        }
    }
}
