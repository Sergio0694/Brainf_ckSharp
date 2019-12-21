using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using Brainf_ck_sharp.Legacy.UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp.Legacy.UWP.PopupService;
using Brainf_ck_sharp.Legacy.UWP.PopupService.Misc;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.DevInfo
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
                LocalizationManager.GetResource("SorryBody"), LocalizationManager.GetResource("SendMail"), "#84007F41".ToColor(), true);
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
            if (result == FlyoutResult.Confirmed) SystemInformation.LaunchStoreForReviewAsync().Forget();
            FlyoutManager.Instance.CloseAllAsync().Forget();
        }
    }
}
