using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.EventArgs;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Helpers.UI;
using Brainf_ck_sharp_UWP.Messages.Requests;
using Brainf_ck_sharp_UWP.Messages.UI;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class LocalSourceCodesBrowserFlyout : UserControl, IAsyncLoadedContent, IEventConfirmedContent<CategorizedSourceCode>
    {
        public LocalSourceCodesBrowserFlyout([CanBeNull] SourceCode code)
        {
            this.InitializeComponent();
            DataContext = new LocalSourceCodesBrowserFlyoutViewModel(code);
            ViewModel.LoadingCompleted += (s, e) =>
            {
                LoadingPending = false;
                LoadingCompleted?.Invoke(this, EventArgs.Empty);
            };
            Unloaded += (s, e) =>
            {
                this.Bindings.StopTracking();
                ViewModel.Cleanup();
                DataContext = null;
                ContentConfirmed = null;
                LoadingCompleted = null;
            };
        }

        public LocalSourceCodesBrowserFlyoutViewModel ViewModel => DataContext.To<LocalSourceCodesBrowserFlyoutViewModel>();

        public event EventHandler<CategorizedSourceCode> ContentConfirmed;

        public CategorizedSourceCode Result { get; private set; }

        private async void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is CategorizedSourceCode item)
            {
                // Ask for confirmation, if needed
                FlyoutResult result = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ProtectUnsavedChanges)) && 
                                      await Messenger.Default.RequestAsync<bool, IDEUnsavedChangesRequestMessage>()
                    ? await FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("UnsavedChangesTitle"),
                        LocalizationManager.GetResource("UnsavedChangesLoading"), LocalizationManager.GetResource("Ok"), stack: true)
                    : FlyoutResult.Confirmed;

                // Load the selected code or cancel
                if (result == FlyoutResult.Canceled) FlyoutManager.Instance.CloseAllAsync().Forget();
                else
                {
                    Messenger.Default.Send(new AppLoadingStatusChangedMessage(true));
                    await Task.Delay(500); // Give some time to the UI to avoid hangs
                    Result = item;
                    ContentConfirmed?.Invoke(this, item);
                }
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
                flyout, null, new Thickness(12, 12, 16, 12), FlyoutDisplayMode.ActualHeight, true);
            if (result == FlyoutResult.Confirmed)
            {
                await ViewModel.RenameItemAsync(e, flyout.Title);
            }
        }

        // Forwards the share event and displays a notification with the result of the share operation
        private async void SavedSourceCodeTemplate_OnShareRequested(object sender, SourceCodeShareEventArgs e)
        {
            if (e.Code == null) return;
            AsyncOperationResult<bool> result = await ViewModel.ShareItemAsync(e.Type, e.Code);
            if (result.Status == AsyncOperationStatus.RunToCompletion && 
                result.Result &&
                (e.Type == SourceCodeShareType.Clipboard || e.Type == SourceCodeShareType.LocalFile))
            {
                NotificationsManager.Instance.ShowNotification(0xEC24.ToSegoeMDL2Icon(), LocalizationManager.GetResource("ShareCompleted"),
                    LocalizationManager.GetResource("ShareCompletedBody"), NotificationType.Default);
            }
            else if (result.Status == AsyncOperationStatus.UnknownErrorHandled ||
                     result.Status == AsyncOperationStatus.Faulted)
            {
                NotificationsManager.Instance.ShowDefaultErrorNotification(LocalizationManager.GetResource("ShareError"), LocalizationManager.GetResource("ShareErrorBody"));
            }
        }

        // Forwards the delete item event
        private void SavedSourceCodeTemplate_OnDeleteRequested(object sender, SourceCode e)
        {
            ViewModel.DeleteItemAsync(e).Forget();
        }

        /// <inheritdoc cref="IAsyncLoadedContent"/>
        public event EventHandler LoadingCompleted;

        /// <inheritdoc cref="IAsyncLoadedContent"/>
        public bool LoadingPending { get; private set; } = true;

        // Asks the user to export the selected code as a C source
        private async void SavedSourceCodeTemplate_OnTranslateToCRequested(object sender, SourceCode e)
        {
            AsyncOperationResult<bool> result = await ViewModel.ExportToCAsync(e);
            if (result.Status == AsyncOperationStatus.RunToCompletion && result.Result)
            {
                NotificationsManager.Instance.ShowNotification(0xEC24.ToSegoeMDL2Icon(), LocalizationManager.GetResource("ExportCompleted"),
                    LocalizationManager.GetResource("ExportCompletedBody"), NotificationType.Default);
            }
        }
    }
}
