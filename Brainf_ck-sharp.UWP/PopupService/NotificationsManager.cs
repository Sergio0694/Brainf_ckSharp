using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.PopupService.UI;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers;
using UICompositionAnimations.Lights;
using DispatcherHelper = Brainf_ck_sharp_UWP.Helpers.DispatcherHelper;

namespace Brainf_ck_sharp_UWP.PopupService
{
    /// <summary>
    /// A static class that manages the in-app notifications
    /// </summary>
    public sealed class NotificationsManager
    {
        /// <summary>
        /// Gets the singleton instance to use to manage the notifications
        /// </summary>
        public static NotificationsManager Instance { get; } = new NotificationsManager();

        // Initializes the singleton instance and subscribes to the messages
        private NotificationsManager()
        {
            Messenger.Default.Register<NotificationCloseRequestMessage>(this, m => CloseNotificationPopupAsync().Forget());
        }

        /// <summary>
        /// The Popup that's currently displayed
        /// </summary>
        private Popup _CurrentPopup;

        /// <summary>
        /// Semaphore to avoid race conditions when setting the current Popup
        /// </summary>
        private readonly SemaphoreSlim NotificationSemaphore = new SemaphoreSlim(1);

        #region Public APIs

        /// <summary>
        /// Shows a notification error with the default icon and settings
        /// </summary>
        /// <param name="title">The title of the notification</param>
        /// <param name="content">The content to show in the notification</param>
        public void ShowDefaultErrorNotification([NotNull] String title, [NotNull] String content)
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
        public void ShowNotification(
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

                // Lights setup
                if (!ApiInformationHelper.IsMobileDevice)
                {
                    bool lightsEnabled = false;
                    PointerPositionSpotLight
                        light = new PointerPositionSpotLight
                        {
                            Z = 30,
                            Shade = 0x80,
                            Active = false
                        },
                        popupLight = new PointerPositionSpotLight
                        {
                            Z = 30,
                            IdAppendage = "[Popup]",
                            Shade = 0x10,
                            Active = false
                        };
                    notificationPopup.Lights.Add(light);
                    notificationPopup.Lights.Add(popupLight);
                    notificationPopup.ManageHostPointerStates((p, value) =>
                    {
                        bool lightsVisible = p == PointerDeviceType.Mouse && value;
                        if (lightsEnabled == lightsVisible) return;
                        light.Active = popupLight.Active = lightsEnabled = lightsVisible;
                    });

                    // Dispose the lights
                    popup.Closed += (s, e) =>
                    {
                        // Dispose the lights
                        notificationPopup.Lights.Clear();
                    };
                }

                // Close the previous notification, if present
                await CloseNotificationPopupAsync();

                // Wait the semaphore
                await NotificationSemaphore.WaitAsync();

                // Local timer to automatically close the notification
                Task.Delay(timespan).ContinueWith(t => CloseNotificationPopupAsync(popup), TaskScheduler.FromCurrentSynchronizationContext()).Forget();

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

        #endregion

        /// <summary>
        /// Closes the current notification, if present
        /// </summary>
        private async Task CloseNotificationPopupAsync()
        {
            await NotificationSemaphore.WaitAsync();
            if (_CurrentPopup?.IsOpen == true)
            {
                await _CurrentPopup.StartCompositionFadeSlideAnimationAsync(1, 0, TranslationAxis.Y, 0, 10, 200, null, 0, EasingFunctionNames.SineEaseOut);
                _CurrentPopup.IsOpen = false;
                _CurrentPopup = null;
            }
            NotificationSemaphore.Release();
        }

        /// <summary>
        /// Close a target notification
        /// </summary>
        /// <param name="popup">The Popup that contains the notification to close</param>
        private async Task CloseNotificationPopupAsync([CanBeNull] Popup popup)
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
