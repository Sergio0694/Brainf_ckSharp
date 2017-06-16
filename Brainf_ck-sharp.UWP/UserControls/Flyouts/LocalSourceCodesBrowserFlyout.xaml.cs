using System;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Enums;
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

        private void SavedSourceCodeTemplate_OnRenameRequested(object sender, SourceCode e)
        {
            throw new NotImplementedException();
        }

        // Forwards the share event and displays a notification with the result of the share operation
        private async void SavedSourceCodeTemplate_OnShareRequested(object sender, (SourceCodeShareType Type, SourceCode Code) e)
        {
            bool result = await ViewModel.ShareItemAsync(e.Type, e.Code);
            if (result)
            {
                NotificationsManager.ShowNotification(0xEC24.ToSegoeMDL2Icon(), LocalizationManager.GetResource("ShareCompleted"),
                    LocalizationManager.GetResource("ShareCompletedBody"), NotificationType.Default);
            }
            else
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
