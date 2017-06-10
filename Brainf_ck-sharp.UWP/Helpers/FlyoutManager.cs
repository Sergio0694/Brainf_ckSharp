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
    public sealed class FlyoutManager
    {
        public static FlyoutManager Instance { get; } = new FlyoutManager();

        private FlyoutManager()
        {
            Messenger.Default.Register<FlyoutCloseRequestMessage>(this, m => TryCloseAsync().Forget());
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += FlyoutManager_VisibleBoundsChanged;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, _) => TryCloseAsync().Forget();
            KeyEventsListener.Esc += (s, _) => TryCloseAsync().Forget();
        }

        // Adjusts the size of the current popup when the window is resized
        private async void FlyoutManager_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            await Semaphore.WaitAsync();
            if (_CurrentPopup != null)
            {
                AdjustPopupSize(_CurrentPopup);
            }
            Semaphore.Release();
        }

        private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);

        private Popup _CurrentPopup;

        public async Task TryCloseAsync()
        {
            await Semaphore.WaitAsync();
            if (_CurrentPopup?.IsOpen == true)
            {
                await _CurrentPopup.StartCompositionFadeScaleAnimationAsync(null, 0, 1, 1.1f, 250, null, null, EasingFunctionNames.CircleEaseOut);
                _CurrentPopup.IsOpen = false;
                _CurrentPopup = null;
            }
            Semaphore.Release();
        }

        private const double MaxPopupHeight = 800;

        private const double MaxPopupWidth = 480;

        public async Task ShowAsync([NotNull] String title, [NotNull] FrameworkElement content)
        {
            await Semaphore.WaitAsync();
            if (_CurrentPopup?.IsOpen == true) _CurrentPopup.IsOpen = false;

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
            

            _CurrentPopup = popup;
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            await popup.StartCompositionFadeScaleAnimationAsync(null, 1, 1.1f, 1, 250, null, null, EasingFunctionNames.CircleEaseOut);
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
                popup.HorizontalOffset = width - MaxPopupWidth / 2;
            }
            if (height <= MaxPopupHeight) popup.Height = height;
            else
            {
                popup.Height = MaxPopupHeight;
                popup.VerticalOffset = height - MaxPopupHeight / 2;
            }
        }
    }
}
