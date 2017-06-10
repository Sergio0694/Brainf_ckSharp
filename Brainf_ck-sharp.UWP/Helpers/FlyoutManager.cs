using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A class that exposes methods to manage flyouts to display across the app
    /// </summary>
    public sealed class FlyoutManager
    {
        #region Constants

        private const double MaxPopupHeight = 800;
        private const double MaxPopupWidth = 480;

        #endregion

        /// <summary>
        /// Gets the singleton instance to use to manage the flyouts
        /// </summary>
        public static FlyoutManager Instance { get; } = new FlyoutManager();

        // Private constructor that initializes the event handlers (can't be a static class due to the Messenger class)
        private FlyoutManager()
        {
            Messenger.Default.Register<FlyoutCloseRequestMessage>(this, m => TryCloseAsync().Forget());
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += FlyoutManager_VisibleBoundsChanged;
            SystemNavigationManager.GetForCurrentView().BackRequested += (_, e) =>
            {
                if (_CurrentPopup != null) e.Handled = true; // Not thread-safe, but anyways
                TryCloseAsync().Forget();
            };
            KeyEventsListener.Esc += (s, _) => TryCloseAsync().Forget();
        }

        // Adjusts the size of the current popup when the window is resized
        private async void FlyoutManager_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            await Semaphore.WaitAsync();
            if (_CurrentPopup != null)
            {
                AdjustPopupSize(_CurrentPopup);
                FlyoutContainer container = _CurrentPopup.Child.To<FlyoutContainer>();
                container.Height = _CurrentPopup.Height;
                container.Width = _CurrentPopup.Width;
            }
            Semaphore.Release();
        }

        // Synchronization semaphore to be thread-safe
        private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);

        // The current popup control
        private Popup _CurrentPopup;

        /// <summary>
        /// Closes the current popup, if there's one displayed
        /// </summary>
        private async Task TryCloseAsync()
        {
            await Semaphore.WaitAsync();
            if (_CurrentPopup?.IsOpen == true)
            {
                Messenger.Default.Send(new FlyoutClosedNotificationMessage());
                await _CurrentPopup.StartCompositionFadeSlideAnimationAsync(null, 0, TranslationAxis.Y, 0, 20, 250, null, null, EasingFunctionNames.CircleEaseOut);
                _CurrentPopup.IsOpen = false;
                _CurrentPopup = null;
            }
            Semaphore.Release();
        }

        /// <summary>
        /// Shows a new flyout with the given parameters
        /// </summary>
        /// <param name="title">The title of the new flyout to show</param>
        /// <param name="content">The content to show inside the flyout</param>
        public async void Show([NotNull] String title, [NotNull] FrameworkElement content)
        {
            // Lock and close the existing popup, if needed
            await Semaphore.WaitAsync();
            Messenger.Default.Send(new FlyoutOpenedMessage());
            if (_CurrentPopup?.IsOpen == true) _CurrentPopup.IsOpen = false;

            // Initialize the container and the target popup
            FlyoutContainer container = new FlyoutContainer
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            container.SetupUI(title, content);
            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                Child = container
            };
            AdjustPopupSize(popup);
            container.Height = popup.Height;
            container.Width = popup.Width;

            // Display and animate the popup
            _CurrentPopup = popup;
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();
        }

        /// <summary>
        /// Adjusts the size of a popup based on the current screen size
        /// </summary>
        /// <param name="popup">The popup to resize</param>
        private static void AdjustPopupSize([NotNull] Popup popup)
        {
            double
                width = ResolutionHelper.CurrentWidth,
                height = ResolutionHelper.CurrentHeight;
            if (width <= MaxPopupWidth) popup.Width = width;
            else
            {
                popup.Width = MaxPopupWidth;
                popup.HorizontalOffset = width / 2 - MaxPopupWidth / 2;
            }
            if (height <= MaxPopupHeight) popup.Height = height;
            else
            {
                popup.Height = MaxPopupHeight;
                popup.VerticalOffset = height / 2 - MaxPopupHeight / 2;
            }
        }
    }
}
