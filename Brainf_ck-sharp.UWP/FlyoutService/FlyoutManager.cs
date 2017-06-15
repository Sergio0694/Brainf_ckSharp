using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.FlyoutService
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
            if (_CurrentPopup != null && _CurrentPopup.Child is FlyoutContainer container)
            {
                AdjustPopupSize(_CurrentPopup, container);
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
        /// <param name="margin">The optional margins to set to the content of the popup to show</param>
        public async void Show([NotNull] String title, [NotNull] FrameworkElement content, [CanBeNull] Thickness? margin = null)
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
            container.SetupUI(title, content, margin);
            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                Child = container
            };
            AdjustPopupSize(popup, container);

            // Display and animate the popup
            _CurrentPopup = popup;
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();
        }

        /// <summary>
        /// Shows a new flyout with the given parameters and waits for a result
        /// </summary>
        /// <param name="title">The title of the new flyout to show</param>
        /// <param name="content">The content to show inside the flyout</param>
        /// <param name="margin">The optional margins to set to the content of the popup to show</param>
        public async Task<FlyoutClosedResult<TEvent>> ShowAsync<TContent, TEvent>([NotNull] String title, [NotNull] TContent content, [CanBeNull] Thickness? margin = null)
            where TContent : FrameworkElement, IEventConfirmedContent<TEvent>
        {
            // Lock and close the existing popup, if needed
            await Semaphore.WaitAsync();
            Messenger.Default.Send(new FlyoutOpenedMessage());
            if (_CurrentPopup?.IsOpen == true) _CurrentPopup.IsOpen = false;

            // Initialize the container and the target popup, and the confirm handler
            FlyoutContainer container = new FlyoutContainer
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            container.SetupUI(title, content, margin);
            TaskCompletionSource<TEvent> tcs = new TaskCompletionSource<TEvent>();
            content.ContentConfirmed += (s, e) =>
            {
                tcs.TrySetResult(e);
                Messenger.Default.Send(new FlyoutCloseRequestMessage());
            };
            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                Child = container
            };
            AdjustPopupSize(popup, container);
            popup.Closed += (s, e) => tcs.TrySetCanceled();

            // Display and animate the popup
            _CurrentPopup = popup;
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();

            // Wait and return the right result
            return await tcs.Task.ContinueWith(t => t.Status == TaskStatus.RanToCompletion
                ? t.Result
                : FlyoutClosedResult<TEvent>.Closed);
        }

        /// <summary>
        /// Adjusts the size of a popup based on the current screen size
        /// </summary>
        /// <param name="popup">The popup to resize</param>
        /// <param name="container">The content hosted inside the <see cref="Popup"/> control</param>
        private static void AdjustPopupSize([NotNull] Popup popup, [NotNull] FrameworkElement container)
        {
            double
                width = ResolutionHelper.CurrentWidth,
                height = ResolutionHelper.CurrentHeight;
            if (width <= MaxPopupWidth)
            {
                container.Width = width;
                popup.HorizontalOffset = 0;
            }
            else
            {
                container.Width = MaxPopupWidth;
                popup.HorizontalOffset = width / 2 - MaxPopupWidth / 2;
            }
            if (height <= MaxPopupHeight)
            {
                container.Height = height;
                popup.VerticalOffset = 0;
            }
            else
            {
                container.Height = MaxPopupHeight;
                popup.VerticalOffset = height / 2 - MaxPopupHeight / 2;
            }
        }
    }
}
