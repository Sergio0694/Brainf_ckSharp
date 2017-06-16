using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.PopupService.UI;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.PopupService
{
    /// <summary>
    /// A static class that manages the in-app notifications
    /// </summary>
    public static class NotificationsManager
    {
        /// <summary>
        /// The Popup that's currently displayed
        /// </summary>
        private static Popup _CurrentPopup;

        /// <summary>
        /// Semaphore to avoid race conditions when setting the current Popup
        /// </summary>
        private static readonly SemaphoreSlim NotificationSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Shows a notification error with the default icon and settings
        /// </summary>
        /// <param name="title">The title of the notification</param>
        /// <param name="content">The content to show in the notification</param>
        public static void ShowDefaultErrorNotification([NotNull] String title, [NotNull] String content)
        {
            ShowNotification(0xE7BA.ToSegoeMDL2Icon(), title, content, NotificationType.Error);
        }

        /// <summary>
        /// Shows a custom notification
        /// </summary>
        /// <param name="icon">The icon of the notification</param>
        /// <param name="title">The title of the new notification to show</param>
        /// <param name="content">The content of the notification</param>
        /// <param name="type">The type of notification to show</param>
        /// <param name="duration">The time interval before the nofitication disappears</param>
        public static void ShowNotification(
            [NotNull] String icon, [NotNull] String title, [NotNull] String content, NotificationType type, TimeSpan? duration = null)
        {
            DispatcherHelper.RunOnUIThreadAsync(async () =>
            {
                // Set the interval to use
                TimeSpan timespan = duration ?? TimeSpan.FromSeconds(3);

                // Prepare the Popup to show
                Popup popup = new Popup
                {
                    VerticalOffset = 20,
                    HorizontalOffset = ResolutionHelper.CurrentWidth - 320,
                };

                // Prepare the notification control
                NotificationPopup notificationPopup = new NotificationPopup(title, icon, content, type);
                popup.Child = notificationPopup;

                // Close the previous notification, if present
                await CloseNotificationPopup(_CurrentPopup);

                // Wait the semaphore
                await NotificationSemaphore.WaitAsync();

                // Local timer to automatically close the notification
                Task.Delay(timespan).ContinueWith(t => CloseNotificationPopup(popup), TaskScheduler.FromCurrentSynchronizationContext()).Forget();

                // Set the current Popup, show it and start its animation
                _CurrentPopup = popup;
                popup.SetVisualOpacity(0);

                // Manage the closed event
                popup.IsOpen = true;
                popup.StartCompositionFadeSlideAnimation(0, 1, TranslationAxis.X, 20, 0, 200, null, 0, EasingFunctionNames.SineEaseOut);

                // Finally release the semaphore
                NotificationSemaphore.Release();
            });
        }

        /// <summary>
        /// Close a target notification
        /// </summary>
        /// <param name="popup">The Popup that contains the notification to close</param>
        private static async Task CloseNotificationPopup([CanBeNull] Popup popup)
        {
            if (popup == null) return;
            await NotificationSemaphore.WaitAsync();
            if ((_CurrentPopup != popup || _CurrentPopup == null) && popup.IsOpen)
            {
                await popup.StartCompositionFadeSlideAnimationAsync(1, 0, TranslationAxis.Y, 0, 10, 200, null, 0, EasingFunctionNames.SineEaseOut);
                popup.IsOpen = false;
                NotificationSemaphore.Release();
                return;
            }
            _CurrentPopup = null;
            await popup.StartCompositionFadeSlideAnimationAsync(1, 0, TranslationAxis.Y, 0, 10, 200, null, 0, EasingFunctionNames.SineEaseOut);
            popup.IsOpen = false;
            NotificationSemaphore.Release();
        }
    }
}
