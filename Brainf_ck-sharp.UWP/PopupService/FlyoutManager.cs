using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.PopupService.UI;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.PopupService
{
    /// <summary>
    /// A class that exposes methods to manage flyouts to display across the app
    /// </summary>
    public sealed class FlyoutManager
    {
        #region Constants

        // Default size
        private const double MaxPopupHeight = 800;
        private const double MaxPopupWidth = 480;

        // Stacked size
        private const double MaxStackedPopupHeight = 740;
        private const double MaxStackedPopupWidth = 420;

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
                if (PopupStack.Count > 0) e.Handled = true; // Not thread-safe, but anyways
                TryCloseAsync().Forget();
            };
            KeyEventsListener.Esc += (s, _) => TryCloseAsync().Forget();
        }

        // Adjusts the size of the current popup when the window is resized
        private async void FlyoutManager_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            await Semaphore.WaitAsync();
            foreach ((FlyoutDisplayInfo info, int i) in PopupStack.Reverse().Select((p, i) => (p, i)))
            {
                AdjustPopupSize(info, i > 0);
            }
            Semaphore.Release();
        }

        // Synchronization semaphore to be thread-safe
        private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);

        // The current popup control
        private Stack<FlyoutDisplayInfo> PopupStack { get; } = new Stack<FlyoutDisplayInfo>();

        /// <summary>
        /// Closes the current popup, if there's one displayed
        /// </summary>
        private async Task TryCloseAsync()
        {
            await Semaphore.WaitAsync();
            if (PopupStack.Count > 0 && PopupStack.Peek().Popup.IsOpen)
            {
                Popup popup = PopupStack.Pop().Popup;
                if (PopupStack.Count == 0) Messenger.Default.Send(new FlyoutClosedNotificationMessage());
                await popup.StartCompositionFadeSlideAnimationAsync(null, 0, TranslationAxis.Y, 0, 20, 250, null, null, EasingFunctionNames.CircleEaseOut);
                popup.IsOpen = false;
            }
            Semaphore.Release();
        }

        /// <summary>
        /// Shows a new flyout with the given parameters
        /// </summary>
        /// <param name="title">The title of the new flyout to show</param>
        /// <param name="content">The content to show inside the flyout</param>
        /// <param name="margin">The optional margins to set to the content of the popup to show</param>
        /// <param name="mode">The desired display mode for the flyout</param>
        /// <param name="stack">Indicates whether or not the popup can be stacked on top of another open popup</param>
        public async Task<FlyoutResult> ShowAsync([NotNull] String title, [NotNull] FrameworkElement content, [CanBeNull] Thickness? margin = null,
            FlyoutDisplayMode mode = FlyoutDisplayMode.ScrollableContent, bool stack = false)
        {
            // Lock and close the existing popup, if needed
            await Semaphore.WaitAsync();
            Messenger.Default.Send(new FlyoutOpenedMessage());
            if (!stack)
            {
                foreach (FlyoutDisplayInfo previous in PopupStack) previous.Popup.IsOpen = false;
                PopupStack.Clear();
            }

            // Initialize the container and the target popup
            FlyoutContainer container = new FlyoutContainer
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Prepare the flyout depending on the desired display mode
            switch (mode)
            {
                case FlyoutDisplayMode.ScrollableContent:
                    container.SetupUI(title, content, margin);
                    break;
                case FlyoutDisplayMode.ActualHeight:
                    double width = CalculateExpectedWidth();
                    if (margin != null) content.Margin = margin.Value;
                    container.SetupFixedUI(title, content, width);
                    break;
                default:
                    throw new ArgumentException("The desired display mode is not valid");
            }

            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                Child = container
            };
            FlyoutDisplayInfo info = new FlyoutDisplayInfo(popup, mode);
            AdjustPopupSize(info, PopupStack.Count > 0);
            TaskCompletionSource<FlyoutResult> tcs = new TaskCompletionSource<FlyoutResult>();
            popup.Closed += (s, e) =>
            {
                tcs.SetResult(container.Confirmed ? FlyoutResult.Confirmed : FlyoutResult.Canceled);
            };

            // Display and animate the popup
            PopupStack.Push(info);
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();
            return await tcs.Task;
        }

        /// <summary>
        /// Shows a new flyout with the given parameters and waits for a result
        /// </summary>
        /// <param name="title">The title of the new flyout to show</param>
        /// <param name="content">The content to show inside the flyout</param>
        /// <param name="margin">The optional margins to set to the content of the popup to show</param>
        /// <param name="mode">The desired display mode for the flyout</param>
        /// <param name="stack">Indicates whether or not the popup can be stacked on top of another open popup</param>
        public async Task<FlyoutClosedResult<TEvent>> ShowAsync<TContent, TEvent>(
            [NotNull] String title, [NotNull] TContent content, [CanBeNull] Thickness? margin = null, 
            FlyoutDisplayMode mode = FlyoutDisplayMode.ScrollableContent, bool stack = false)
            where TContent : FrameworkElement, IEventConfirmedContent<TEvent>
        {
            // Lock and close the existing popup, if needed
            await Semaphore.WaitAsync();
            Messenger.Default.Send(new FlyoutOpenedMessage());
            if (!stack)
            {
                foreach (FlyoutDisplayInfo previous in PopupStack) previous.Popup.IsOpen = false;
                PopupStack.Clear();
            }

            // Initialize the container and the target popup, and the confirm handler
            FlyoutContainer container = new FlyoutContainer
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Prepare the flyout depending on the desired display mode
            switch (mode)
            {
                case FlyoutDisplayMode.ScrollableContent:
                    container.SetupUI(title, content, margin);
                    break;
                case FlyoutDisplayMode.ActualHeight:
                    double width = CalculateExpectedWidth();
                    if (margin != null) content.Margin = margin.Value;
                    container.SetupFixedUI(title, content, width);
                    break;
                default:
                    throw new ArgumentException("The desired display mode is not valid");
            }

            // Setup the completion events and manage the popup size
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
            FlyoutDisplayInfo info = new FlyoutDisplayInfo(popup, mode);
            AdjustPopupSize(info, PopupStack.Count > 0);
            popup.Closed += (s, e) => tcs.TrySetCanceled();

            // Display and animate the popup
            PopupStack.Push(info);
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
        /// Calculates the expected width for a flyout to display
        /// </summary>
        private static double CalculateExpectedWidth()
        {
            double width = ResolutionHelper.CurrentWidth;
            return width <= MaxPopupWidth ? width : MaxPopupWidth;
        }

        /// <summary>
        /// Adjusts the size of a popup based on the current screen size
        /// </summary>
        /// <param name="info">The wrapped info on the popup to resize and its content</param>
        /// <param name="stacked">Indicates whether or not the current popup is not the first one being displayed</param>
        private static void AdjustPopupSize([NotNull] FlyoutDisplayInfo info, bool stacked)
        {
            // Calculate the current parameters
            double
                width = ResolutionHelper.CurrentWidth,
                height = ResolutionHelper.CurrentHeight,
                maxWidth = stacked ? MaxStackedPopupWidth : MaxPopupWidth,
                maxHeight = stacked ? MaxStackedPopupHeight : MaxPopupHeight,
                margin = UniversalAPIsHelper.IsMobileDevice ? 0 : 24; // The minimum margin to the edges of the screen

            // Update the width first
            if (width - margin <= maxWidth) info.Container.Width = width - margin;
            else info.Container.Width = maxWidth - margin;
            info.Popup.HorizontalOffset = width / 2 - info.Container.Width / 2;

            // Calculate the height depending on the display mode
            if (info.DisplayMode == FlyoutDisplayMode.ScrollableContent)
            {
                if (height - margin <= maxHeight) info.Container.Height = height - margin;
                else info.Container.Height = maxHeight;
                info.Popup.VerticalOffset = height / 2 - info.Container.Height / 2;
            }
            else
            {
                // Calculate the desired size and arrange the popup
                Size desired = info.Container.CalculateDesiredSize();
                if (desired.Height <= height + margin)
                {
                    info.Container.Height = desired.Height;
                }
                else info.Container.Height = height - margin;
                info.Popup.VerticalOffset = (height / 2 - info.Container.Height / 2) / 2;
            }
        }

        /// <summary>
        /// Shows a given content inside a popup with an animation and offset similar of an attached Flyout
        /// </summary>
        /// <param name="content">The control to show</param>
        /// <param name="rect">The target area to try not to cover</param>
        /// <param name="tryCenter">Indicates whether or not to try to center the popup to the source control</param>
        public static void ShowCustomContextFlyout([NotNull] FrameworkElement content, Rect rect, bool tryCenter = false)
        {
            // Calculate the target size
            content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (content.DesiredSize.Width > ResolutionHelper.CurrentWidth - 24)
            {
                content.Width = ResolutionHelper.CurrentWidth;
                content.Measure(new Size(content.Width, double.PositiveInfinity));
                content.Height = content.DesiredSize.Height;
            }
            else if (content.DesiredSize.Height > ResolutionHelper.CurrentHeight - 24)
            {
                content.Height = ResolutionHelper.CurrentHeight;
                content.Measure(new Size(double.PositiveInfinity, content.Height));
                content.Width = content.DesiredSize.Width;
            }
            else
            {
                content.Width = content.DesiredSize.Width;
                content.Height = content.DesiredSize.Height;
            }

            // Adjust the display size and position
            (double, double) CalculateSizeAndOffset()
            {
                // Calculate the final offset
                double x = 0, y = 0;
                if (content.Height <= rect.Top - 8)
                {
                    y = rect.Top - content.Height - 8;
                }
                else if (content.Height + 8 < ResolutionHelper.CurrentHeight)
                {
                    y = 8;
                }
                if (content.Width < ResolutionHelper.CurrentWidth - rect.Left)
                {
                    x = rect.Left;
                }
                else if (content.Width < ResolutionHelper.CurrentWidth)
                {
                    x = ResolutionHelper.CurrentWidth - content.Width;
                }

                // Shift the target position left if needed
                if (tryCenter)
                {
                    double
                        offset = content.Width / 2,
                        half = rect.Width / 2;
                    if (x > offset - half) x -= offset - half;
                    else if (x > 8) x = 8;
                }
                return (x, y);
            }

            // Create the new popup
            (double hx, double vy) = CalculateSizeAndOffset();
            Popup popup = new Popup
            {
                IsLightDismissEnabled = true,
                HorizontalOffset = hx,
                VerticalOffset = vy
            };

            // Create the grid with the content and its drop shadow
            Grid parent = new Grid();
            Grid grid = new Grid();
            parent.Children.Add(grid);

            // Build the shadow frame and insert the actual popup content
            foreach ((float x, float y) in new (float, float)[]
            {
                ((float)content.Width, 0),
                (-(float)content.Width, 0),
                (0, -(float)content.Height),
                (0, (float)content.Height)
            })
            {
                // Setup the shadow
                Border border = new Border();
                grid.Children.Add(border);
                content.AttachVisualShadow(border, true, 
                    (float)content.Width + 1, (float)content.Height + 1,
                    Colors.Black, 1, -0.5f, -0.5f, null, x, y);
            }
            grid.Children.Add(content);

            // Assign the popup content
            popup.Child = parent;
            grid.SetVisualOpacity(0);
            popup.IsOpen = true;
            grid.StartCompositionFadeSlideAnimation(0, 1, TranslationAxis.Y, 20, 0, 200, null, null, EasingFunctionNames.CircleEaseOut);
            grid.SetCompositionFadeSlideImplicitAnimation(ImplicitAnimationType.Hide, 1, 0, TranslationAxis.Y, 0, 8, 250, null, null, EasingFunctionNames.CircleEaseOut);
        }
    }
}
