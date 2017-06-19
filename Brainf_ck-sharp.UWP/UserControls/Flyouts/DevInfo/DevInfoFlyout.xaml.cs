using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo
{
    public sealed partial class DevInfoFlyout : UserControl
    {
        public DevInfoFlyout()
        {
            this.InitializeComponent();
        }

        // The current ProductId for Brainf*ck#
        private const String ProductId = "9nblgggzhvq5";

        // Opens the Store review page for the app
        private void RateStoreButton_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={ProductId}")).AsTask().Forget();
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
    }
}
