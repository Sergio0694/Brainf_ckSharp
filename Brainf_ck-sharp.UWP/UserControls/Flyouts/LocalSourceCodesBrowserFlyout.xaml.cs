using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.FlyoutService;
using Brainf_ck_sharp_UWP.FlyoutService.Interfaces;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.ViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class LocalSourceCodesBrowserFlyout : UserControl, IEventConfirmedContent<SourceCode>
    {
        public LocalSourceCodesBrowserFlyout()
        {
            this.InitializeComponent();
            DataContext = new LocalSourceCodesBrowserFlyoutViewModel();
        }

        public LocalSourceCodesBrowserFlyoutViewModel ViewModel => DataContext.To<LocalSourceCodesBrowserFlyoutViewModel>();

        public event EventHandler<SourceCode> ContentConfirmed;

        public SourceCode Result { get; private set; }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SourceCode code)
            {
                Result = code;
                ContentConfirmed?.Invoke(this, code);
            }
        }

        // Forwards the favorite toggle event
        private void SavedSourceCodeTemplate_OnFavoriteToggleRequested(object sender, SourceCode e)
        {
            ViewModel.ToggleFavorite(e).Forget();
        }

        // Forwards a request to rename a source code
        private async void SavedSourceCodeTemplate_OnRenameRequested(object sender, SourceCode e)
        {
            SaveCodePromptFlyout flyout = new SaveCodePromptFlyout(e.Code, e.Title);
            FlyoutResult result = await FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("RenameCode"), 
                flyout, new Thickness(12, 12, 16, 12), FlyoutDisplayMode.ActualHeight, true);
            if (result == FlyoutResult.Confirmed)
            {
                await ViewModel.RenameItemAsync(e, flyout.Title);
            }
        }

        // Forwards the share event and displays a notification with the result of the share operation
        private async void SavedSourceCodeTemplate_OnShareRequested(object sender, (SourceCodeShareType Type, SourceCode Code) e)
        {
            AsyncOperationResult<bool> result = await ViewModel.ShareItemAsync(e.Type, e.Code);
            if (result.Status == AsyncOperationStatus.RunToCompletion && result.Result)
            {
                NotificationsManager.ShowNotification(0xEC24.ToSegoeMDL2Icon(), LocalizationManager.GetResource("ShareCompleted"),
                    LocalizationManager.GetResource("ShareCompletedBody"), NotificationType.Default);
            }
            else if (result.Status != AsyncOperationStatus.Canceled)
            {
                NotificationsManager.ShowDefaultErrorNotification(LocalizationManager.GetResource("ShareError"), LocalizationManager.GetResource("ShareErrorBody"));
            }
        }

        // Forwards the delete item event
        private void SavedSourceCodeTemplate_OnDeleteRequested(object sender, SourceCode e)
        {
            ViewModel.DeleteItemAsync(e).Forget();
        }
    }
}
