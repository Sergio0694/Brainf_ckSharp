using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Microsoft.Toolkit.Uwp;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo
{
    public sealed partial class ReviewPromptFlyout : UserControl
    {
        public ReviewPromptFlyout()
        {
            this.InitializeComponent();
        }

        // Prompts the user to send a feedback email
        private async void SadButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutResult result = await FlyoutManager.Instance.ShowAsync($"😥 {LocalizationManager.GetResource("SorryTitle")}",
                LocalizationManager.GetResource("SorryBody"), LocalizationManager.GetResource("SendMail"), "#80CE5C00".ToColor(), true);
            if (result == FlyoutResult.Confirmed)
            {
                await EmailHelper.SendFeedbackEmail();
            }
            FlyoutManager.Instance.CloseAllAsync().Forget();

        }

        // Prompts the user to give the app a review in the Store
        private async void HappyButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutResult result = await FlyoutManager.Instance.ShowAsync($"🐱‍💻 {LocalizationManager.GetResource("ARealNinjacat")}",
                LocalizationManager.GetResource("ARealNinjacatBody"), LocalizationManager.GetResource("Sure"), null, true);
            if (result == FlyoutResult.Confirmed)
            {
                await LauncherHelper.OpenStoreAppReviewPageAsync();
            }
            FlyoutManager.Instance.CloseAllAsync().Forget();
        }
    }
}
