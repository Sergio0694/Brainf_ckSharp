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
using Windows.UI.Xaml.Media;
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
        /// Disables the currently open flyouts so that the user can't interact with them as long as there's another one on top of them
        /// </summary>
        private void DisableOpenFlyouts()
        {
            foreach (FlyoutDisplayInfo info in PopupStack) info.Popup.IsHitTestVisible = false;
        }

        /// <summary>
        /// Closes the current popup, if there's one displayed
        /// </summary>
        private async Task TryCloseAsync()
        {
            await Semaphore.WaitAsync();
            if (PopupStack.Count > 0 && PopupStack.Peek().Popup.IsOpen)
            {
                // Close the current popup
                Popup popup = PopupStack.Pop().Popup;
                if (PopupStack.Count == 0) Messenger.Default.Send(new FlyoutClosedNotificationMessage());
                await popup.StartCompositionFadeSlideAnimationAsync(null, 0, TranslationAxis.Y, 0, 20, 250, null, null, EasingFunctionNames.CircleEaseOut);
                popup.IsOpen = false;

                // Resto the previous popup, if present
                if (PopupStack.Count > 0) PopupStack.Peek().Popup.IsHitTestVisible = true;
            }
            Semaphore.Release();
        }

        /// <summary>
        /// Closes all the currently displayed popups
        /// </summary>
        public async Task CloseAllAsync()
        {
            await Semaphore.WaitAsync();
            if (PopupStack.Count > 0)
            {
                while (PopupStack.Count > 0)
                {
                    Popup popup = PopupStack.Pop().Popup;
                    popup.StartCompositionFadeSlideAnimation(null, 0, TranslationAxis.Y, 0, 20, 250, null, null,
                        EasingFunctionNames.CircleEaseOut, () => popup.IsOpen = false);
                    await Task.Delay(80);
                }
                Messenger.Default.Send(new FlyoutClosedNotificationMessage());
            }
            Semaphore.Release();
        }

        /// <summary>
        /// Shows a simple message dialog with a title and a content
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="message">The message to show to the user</param>
        public void Show([NotNull] String title, [NotNull] String message)
        {
            // Prepare the message and show it inside a popup
            TextBlock block = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };
            ShowAsync(title, block, new Thickness(12, 12, 16, 12), FlyoutDisplayMode.ActualHeight).Forget();
        }

        /// <summary>
        /// Shows a new flyout with a text content and a confirm button
        /// </summary>
        /// <param name="title">The title of the new flyout to show</param>
        /// <param name="message">The text to show inside the flyout</param>
        /// <param name="confirm">The text to display in the confirm button</param>
        /// <param name="color">The optional override color for the confirm button</param>
        /// <param name="stack">Indicates whether or not the popup can be stacked on top of another open popup</param>
        public async Task<FlyoutResult> ShowAsync([NotNull] String title, [NotNull] String message, [NotNull] String confirm, 
            [CanBeNull] Color? color = null, bool stack = false)
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
            FlyoutContainer container = new FlyoutContainer(null, null)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Prepare the flyout depending on the desired display mode
            double width = CalculateExpectedWidth();
            TextBlock block = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Margin = new Thickness(12, 12, 16, 12)
            };
            container.SetupFixedUI(title, block, width);
            container.SetupUI(confirm, color);

            // Create the popup to display
            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                Child = container
            };
            FlyoutDisplayInfo info = new FlyoutDisplayInfo(popup, FlyoutDisplayMode.ActualHeight);
            AdjustPopupSize(info, PopupStack.Count > 0);
            TaskCompletionSource<FlyoutResult> tcs = new TaskCompletionSource<FlyoutResult>();

            // Setup the closed handler
            void CloseHandler(object s, object e)
            {
                tcs.SetResult(container.Confirmed ? FlyoutResult.Confirmed : FlyoutResult.Canceled);
                popup.Child = null;
                popup.Closed -= CloseHandler;
            }
            popup.Closed += CloseHandler;

            // Display and animate the popup
            DisableOpenFlyouts();
            PopupStack.Push(info);
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();
            return await tcs.Task;
        }

        /// <summary>
        /// Shows a new flyout with the given parameters
        /// </summary>
        /// <param name="title">The title of the new flyout to show</param>
        /// <param name="content">The content to show inside the flyout</param>
        /// <param name="margin">The optional margins to set to the content of the popup to show</param>
        /// <param name="mode">The desired display mode for the flyout</param>
        /// <param name="stack">Indicates whether or not the popup can be stacked on top of another open popup</param>
        /// <param name="openCallback">An optional callback to invoke when the popup is displayed</param>
        /// <param name="background">The optional custom background tint color for the popup to display</param>
        /// <param name="tintMix">The optional custom background tint color mix for the popup to display</param>
        public async Task<FlyoutResult> ShowAsync([NotNull] String title, [NotNull] FrameworkElement content, [CanBeNull] Thickness? margin = null,
            FlyoutDisplayMode mode = FlyoutDisplayMode.ScrollableContent, bool stack = false, [CanBeNull] Action openCallback = null,
            Color? background = null, float? tintMix = null)
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
            FlyoutContainer container = new FlyoutContainer(background, tintMix)
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

            // Create the popup to display
            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                Child = container
            };
            FlyoutDisplayInfo info = new FlyoutDisplayInfo(popup, mode);
            AdjustPopupSize(info, PopupStack.Count > 0);
            TaskCompletionSource<FlyoutResult> tcs = new TaskCompletionSource<FlyoutResult>();

            // Setup the closed handler
            void CloseHandler(object s, object e)
            {
                tcs.SetResult(container.Confirmed ? FlyoutResult.Confirmed : FlyoutResult.Canceled);
                popup.Child = null;
                popup.Closed -= CloseHandler;
            }
            popup.Closed += CloseHandler;

            // Display and animate the popup
            DisableOpenFlyouts();
            PopupStack.Push(info);
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();
            openCallback?.Invoke();
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
        /// <param name="openCallback">An optional callback to invoke when the popup is displayed</param>
        /// <param name="background">The optional custom background tint color for the popup to display</param>
        /// <param name="tintMix">The optional custom background tint color mix for the popup to display</param>
        public async Task<FlyoutClosedResult<TEvent>> ShowAsync<TContent, TEvent>(
            [NotNull] String title, [NotNull] TContent content, [CanBeNull] Thickness? margin = null, 
            FlyoutDisplayMode mode = FlyoutDisplayMode.ScrollableContent, bool stack = false, [CanBeNull] Action openCallback = null,
            Color? background = null, float? tintMix = null)
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
            FlyoutContainer container = new FlyoutContainer(background, tintMix)
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

            // Prepare the closed handler
            void CloseHandler(object s, object e)
            {
                tcs.TrySetCanceled();
                popup.Child = null;
                popup.Closed -= CloseHandler;
            }
            popup.Closed += CloseHandler;

            // Display and animate the popup
            DisableOpenFlyouts();
            PopupStack.Push(info);
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();
            openCallback?.Invoke();

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
                screenWidth = ResolutionHelper.CurrentWidth,
                screenHeight = ResolutionHelper.CurrentHeight,
                maxWidth = stacked ? MaxStackedPopupWidth : MaxPopupWidth,
                maxHeight = stacked ? MaxStackedPopupHeight : MaxPopupHeight,
                margin = UniversalAPIsHelper.IsMobileDevice ? 12 : 24; // The minimum margin to the edges of the screen

            // Update the width first
            if (screenWidth - margin <= maxWidth) info.Container.Width = screenWidth - (UniversalAPIsHelper.IsMobileDevice ? 0 : margin);
            else info.Container.Width = maxWidth - margin;
            info.Popup.HorizontalOffset = screenWidth / 2 - info.Container.Width / 2;

            // Calculate the height depending on the display mode
            if (info.DisplayMode == FlyoutDisplayMode.ScrollableContent)
            {
                // Edge case for tiny screens not on mobile phones
                if (!UniversalAPIsHelper.IsMobileDevice && screenHeight < 400)
                {
                    info.Container.Height = screenHeight;
                    info.Popup.VerticalOffset = 0;
                }
                else
                {
                    // Calculate and adjust the right popup height
                    info.Container.Height = screenHeight - margin <= maxHeight
                        ? screenHeight - (UniversalAPIsHelper.IsMobileDevice ? 0 : margin)
                        : maxHeight;
                    info.Popup.VerticalOffset = screenHeight / 2 - info.Container.Height / 2;
                }
            }
            else
            {
                // Calculate the desired size and arrange the popup
                Size desired = info.Container.CalculateDesiredSize();
                info.Container.Height = desired.Height <= screenHeight + margin 
                    ? desired.Height 
                    : screenHeight - (UniversalAPIsHelper.IsMobileDevice ? 0 : margin);
                info.Popup.VerticalOffset = (screenHeight / 2 - info.Container.Height / 2) / 2;
            }
        }

        /// <summary>
        /// Shows a given content inside a popup with an animation and offset similar of an attached Flyout
        /// </summary>
        /// <param name="content">The control to show</param>
        /// <param name="rect">The target area to try not to cover</param>
        /// <param name="tryCenter">Indicates whether or not to try to center the popup to the source control</param>
        public async void ShowCustomContextFlyout([NotNull] FrameworkElement content, Rect rect, bool tryCenter = false)
        {
            // Close existing popups if needed
            await Semaphore.WaitAsync();
            if (PopupStack.Count > 0)
            {
                foreach (FlyoutDisplayInfo info in PopupStack) info.Popup.IsOpen = false;
                Messenger.Default.Send(new FlyoutClosedNotificationMessage());
            }
            foreach (Popup pending in VisualTreeHelper.GetOpenPopups(Window.Current)) pending.IsOpen = false;

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
                IsLightDismissEnabled = false,
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

            // Setup fade in and out animations
            Grid hitGrid = new Grid
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Height = ResolutionHelper.CurrentHeight,
                Width = ResolutionHelper.CurrentWidth
            };
            Popup hit = new Popup
            {
                Child = hitGrid,
                IsLightDismissEnabled = false,
                IsOpen = true
            };
            popup.IsOpen = true; // Open the context menu popup on top of the hit target
            void ClosePopups()
            {
                // The manual animation here is a workaround for a crash with the implicit hide composition animations
                hit.IsOpen = false;
                grid.IsHitTestVisible = false;
                grid.StartCompositionFadeSlideAnimation(1, 0, TranslationAxis.Y, 0, 8, 200, null, null, EasingFunctionNames.CircleEaseOut,
                    () => popup.IsOpen = false);
            }
            hitGrid.Tapped += (_, e) => ClosePopups();
            void WindowSizeHandler(object s, WindowSizeChangedEventArgs e)
            {
                Window.Current.SizeChanged -= WindowSizeHandler;
                ClosePopups();
            }
            Window.Current.SizeChanged += WindowSizeHandler;
            grid.StartCompositionFadeSlideAnimation(0, 1, TranslationAxis.Y, 20, 0, 200, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();
        }
    }
}
