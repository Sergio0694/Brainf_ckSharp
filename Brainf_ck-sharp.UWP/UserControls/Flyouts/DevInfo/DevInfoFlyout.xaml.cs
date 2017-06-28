using System;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo
{
    public sealed partial class DevInfoFlyout : UserControl
    {
        public DevInfoFlyout()
        {
            this.InitializeComponent();
            BuildBlock.Text = AppSettingsManager.AppVersion;
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

        // Show the donation options
        private async void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            Donations.DevSupportPickerFlyout flyout = new Donations.DevSupportPickerFlyout();
            FlyoutClosedResult<int> result = await FlyoutManager.Instance.ShowAsync<Donations.DevSupportPickerFlyout, int>(
                LocalizationManager.GetResource("Donate"), flyout, new Thickness(), FlyoutDisplayMode.ActualHeight, true);
            if (result) ProcessDonationAsync(result.Value).Forget();
        }

        // In-app products
        private const String CoffeeInAppStoreId = "9mtm2n891932";
        private const String PresentInAppStoreId = "9ntlgb7lh7kq";
        private const String DonateInAppStoreId = "9mvdsn65qdgh";
        private const String VIPSupportInAppStoreId = "9p4ws1x7h5s8";

        /// <summary>
        /// Processes a donation in the Store
        /// </summary>
        /// <param name="option">The index of the target IAP to buy</param>
        public static async Task ProcessDonationAsync(int option)
        {
            // Get the id of the product to purchase
            String id;
            switch (option)
            {
                case 0: id = CoffeeInAppStoreId; break;
                case 1: id = PresentInAppStoreId; break;
                case 2: id = DonateInAppStoreId; break;
                case 3: id = VIPSupportInAppStoreId; break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Try to purchase the item
            Messenger.Default.Send(new AppLoadingStatusChangedMessage(true));
            StorePurchaseResult result;
            StoreContext store = StoreContext.GetDefault();
            try
            {
                result = await store.RequestPurchaseAsync(id);
            }
            catch
            {
                NotificationsManager.ShowDefaultErrorNotification(
                    LocalizationManager.GetResource("StoreConnectionError"), LocalizationManager.GetResource("StoreConnectionErrorBody"));
                return;
            }
            finally
            {
                Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
            }

            // Display the result
            switch (result.Status)
            {
                case StorePurchaseStatus.Succeeded:
                    NotificationsManager.ShowNotification(0xEC24.ToSegoeMDL2Icon(), LocalizationManager.GetResource("DonationCompleted"), 
                        LocalizationManager.GetResource("DonationCompletedBody"), NotificationType.Default);
                    store.ReportConsumableFulfillmentAsync(id, 1, Guid.NewGuid()).AsTask().Forget();
                    break;
                case StorePurchaseStatus.NotPurchased:
                    NotificationsManager.ShowDefaultErrorNotification(LocalizationManager.GetResource("PurchaseCanceled"), LocalizationManager.GetResource("PurchaseCanceledBody"));
                    break;
                case StorePurchaseStatus.AlreadyPurchased:
                    store.ReportConsumableFulfillmentAsync(id, 1, Guid.NewGuid()).AsTask().Forget();
                    NotificationsManager.ShowDefaultErrorNotification($"{LocalizationManager.GetResource("SomethingBadHappened")} :'(", LocalizationManager.GetResource("DonationErrorBody"));
                    break;
                default:
                    // Error
                    NotificationsManager.ShowDefaultErrorNotification($"{LocalizationManager.GetResource("SomethingBadHappened")} :'(", LocalizationManager.GetResource("DonationErrorBody"));
                    break;
            }
        }

        // Shows the changelog flyout
        private void ShowChangelogButton_Click(object sender, RoutedEventArgs e)
        {
            ChangelogViewFlyout flyout = new ChangelogViewFlyout();
            Task.Delay(100).ContinueWith(t => flyout.ViewModel.LoadGroupsAsync(), TaskScheduler.FromCurrentSynchronizationContext()).Forget();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("Changelog"), flyout, new Thickness(),
                FlyoutDisplayMode.ScrollableContent, true).Forget();
        }
    }
}
