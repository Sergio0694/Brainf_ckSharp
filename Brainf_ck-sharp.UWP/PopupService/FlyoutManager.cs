using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using Brainf_ck_sharp_UWP.Messages.KeyboardShortcuts;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.PopupService.InternalTypes;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.PopupService.UI;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Lights;
using UICompositionAnimations.XAMLTransform;

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

        /// <summary>
        /// Gets an action that closes the current custom context menu, if present
        /// </summary>
        [CanBeNull]
        private Action _CloseContextMenu;

        // Private constructor that initializes the event handlers (can't be a static class due to the Messenger class)
        private FlyoutManager()
        {
            Messenger.Default.Register<FlyoutCloseRequestMessage>(this, m => TryCloseAsync().Forget());
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += FlyoutManager_VisibleBoundsChanged;
            SystemNavigationManager.GetForCurrentView().BackRequested += (_, e) =>
            {
                if (PopupStack.Count > 0) e.Handled = true; // Not thread-safe, but anyways
                if (_CloseContextMenu != null)
                {
                    e.Handled = true;
                    _CloseContextMenu?.Invoke();
                }
                else TryCloseAsync().Forget();
            };
            Messenger.Default.Register<EscKeyPressedMessage>(this, _ =>
            {
                if (_CloseContextMenu != null) _CloseContextMenu?.Invoke();
                else TryCloseAsync().Forget();
            });
        }

        // Adjusts the size of the current popup when the window is resized
        private async void FlyoutManager_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            await Semaphore.WaitAsync();
            int i = 0;
            foreach (FlyoutDisplayInfo info in PopupStack.Reverse())
            {
                AdjustPopupSize(info, i > 0);
                i++;
            }
            Semaphore.Release();
        }

        // Synchronization semaphore to be thread-safe
        private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);

        // The current popup control
        private Stack<FlyoutDisplayInfo> PopupStack { get; } = new Stack<FlyoutDisplayInfo>();

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

        #region Tools

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
        /// <param name="animateHeight">Indicates whether or not to animate the height resize</param>
        private static void AdjustPopupSize([NotNull] FlyoutDisplayInfo info, bool stacked, bool animateHeight = false)
        {
            // Calculate the current parameters
            double
                screenWidth = ResolutionHelper.CurrentWidth,
                screenHeight = ResolutionHelper.CurrentHeight,
                maxWidth = stacked ? MaxStackedPopupWidth : MaxPopupWidth,
                maxHeight = stacked ? MaxStackedPopupHeight : MaxPopupHeight,
                margin = 24; // The minimum margin to the edges of the screen

            // Update the width first
            if (screenWidth - margin <= maxWidth) info.Container.Width = screenWidth - margin;
            else info.Container.Width = maxWidth - margin;
            info.Popup.HorizontalOffset = screenWidth / 2 - info.Container.Width / 2;

            // Calculate the height depending on the display mode
            if (info.DisplayMode == FlyoutDisplayMode.ScrollableContent)
            {
                // Edge case for tiny screens not on mobile phones
                if (screenHeight < 400) info.Container.Height = screenHeight;
                else
                {
                    // Calculate and adjust the right popup height
                    info.Container.Height = screenHeight - margin <= maxHeight // The expanded popup will cover the whole screen
                        ? screenHeight - margin
                        : maxHeight;
                    info.Popup.VerticalOffset = screenHeight / 2 - info.Container.Height / 2;
                }
            }
            else
            {
                // Calculate the desired size and arrange the popup
                Size desired = info.Container.CalculateDesiredSize();
                double height = desired.Height <= screenHeight + margin
                    ? desired.Height
                    : screenHeight - margin;
                if (animateHeight)
                {
                    XAMLTransformToolkit.CreateDoubleAnimation(info.Container, "Height", null, height,
                        100, EasingFunctionNames.CircleEaseOut, true).ToStoryboard().Begin();
                }
                else info.Container.Height = height;
                info.Popup.VerticalOffset = (screenHeight / 2 - info.Container.Height / 2) / 2;
            }
        }

        /// <summary>
        /// Prepares a flyout container and its lights
        /// </summary>
        private static Tuple<FlyoutContainer, Action> SetupFlyoutContainer()
        {
            // Initialize the container and the target popup
            FlyoutContainer container = new FlyoutContainer
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Lights setup
            LightsSourceHelper.SetIsLightsContainer(container, true);

            // Return the results
            return Tuple.Create<FlyoutContainer, Action>(container, () =>
            {
                // Dispose the lights
                LightsSourceHelper.SetIsLightsContainer(container, false);
            });
        }

        #endregion

        #region Flyout APIs

        /// <summary>
        /// Checks whether or not there's at least one visible dialog
        /// </summary>
        public async Task<bool> IsFlyoutOpenAsync()
        {
            await Semaphore.WaitAsync();
            try
            {
                return PopupStack.TryPeek(out _);
            }
            finally
            {
                Semaphore.Release(); // Wouldn't want to forget this!
            }
        }

        /// <summary>
        /// Shows a simple message dialog with a title and a content
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="message">The message to show to the user</param>
        public void Show([NotNull] string title, [NotNull] string message)
        {
            // Prepare the message and show it inside a popup
            TextBlock block = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };
            ShowAsync(title, block, null, new Thickness(12, 12, 16, 12), FlyoutDisplayMode.ActualHeight).Forget();
        }

        /// <summary>
        /// Shows a new flyout with a text content and a confirm button
        /// </summary>
        /// <param name="title">The title of the new flyout to show</param>
        /// <param name="message">The text to show inside the flyout</param>
        /// <param name="confirm">The text to display in the confirm button</param>
        /// <param name="color">The optional override color for the confirm button</param>
        /// <param name="stack">Indicates whether or not the popup can be stacked on top of another open popup</param>
        public async Task<FlyoutResult> ShowAsync([NotNull] string title, [NotNull] string message, [NotNull] string confirm, 
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
            Tuple<FlyoutContainer, Action> setup = SetupFlyoutContainer();

            // Prepare the flyout depending on the desired display mode
            double width = CalculateExpectedWidth();
            TextBlock block = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Margin = new Thickness(12, 12, 16, 12)
            };
            setup.Item1.SetupFixedUI(title, block, width);
            setup.Item1.SetupButtonsUI(confirm, color);

            // Create the popup to display
            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                Child = setup.Item1
            };
            FlyoutDisplayInfo info = new FlyoutDisplayInfo(popup, FlyoutDisplayMode.ActualHeight);
            AdjustPopupSize(info, PopupStack.Count > 0);
            TaskCompletionSource<FlyoutResult> tcs = new TaskCompletionSource<FlyoutResult>();

            // Setup the closed handler
            void CloseHandler(object s, object e)
            {
                // Results
                tcs.SetResult(setup.Item1.Confirmed ? FlyoutResult.Confirmed : FlyoutResult.Canceled);
                popup.Child = null;
                popup.Closed -= CloseHandler;
                setup.Item2?.Invoke();
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
        /// <param name="confirm">The optional text to display in the confirm button</param>
        /// <param name="margin">The optional margins to set to the content of the popup to show</param>
        /// <param name="mode">The desired display mode for the flyout</param>
        /// <param name="stack">Indicates whether or not the popup can be stacked on top of another open popup</param>
        /// <param name="openCallback">An optional callback to invoke when the popup is displayed</param>
        public async Task<FlyoutResult> ShowAsync(
            [NotNull] string title, [NotNull] FrameworkElement content, [CanBeNull] string confirm = null, 
            [CanBeNull] Thickness? margin = null, FlyoutDisplayMode mode = FlyoutDisplayMode.ScrollableContent, 
            bool stack = false, [CanBeNull] Action openCallback = null)
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
            Tuple<FlyoutContainer, Action> setup = SetupFlyoutContainer();

            // Prepare the flyout depending on the desired display mode
            switch (mode)
            {
                case FlyoutDisplayMode.ScrollableContent:
                    setup.Item1.SetupUI(title, content, margin);
                    break;
                case FlyoutDisplayMode.ActualHeight:
                    double width = CalculateExpectedWidth();
                    if (margin != null) content.Margin = margin.Value;
                    setup.Item1.SetupFixedUI(title, content, width);
                    break;
                default:
                    throw new ArgumentException("The desired display mode is not valid");
            }
            if (confirm != null) setup.Item1.SetupButtonsUI(confirm, null); // Setup the confirm button

            // Create the popup to display
            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                Child = setup.Item1
            };
            FlyoutDisplayInfo info = new FlyoutDisplayInfo(popup, mode);
            TaskCompletionSource<FlyoutResult> tcs = new TaskCompletionSource<FlyoutResult>();

            // Setup the closed handler
            void CloseHandler(object s, object e)
            {
                // Results
                tcs.SetResult(setup.Item1.Confirmed ? FlyoutResult.Confirmed : FlyoutResult.Canceled);
                popup.Child = null;
                popup.Closed -= CloseHandler;
                setup.Item2?.Invoke();
            }
            popup.Closed += CloseHandler;

            // Display and animate the popup
            DisableOpenFlyouts();
            PopupStack.Push(info);
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            AdjustPopupSize(info, PopupStack.Count > 1);
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            if (mode == FlyoutDisplayMode.ActualHeight) AdjustPopupSize(info, PopupStack.Count > 1, true);
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
        public async Task<FlyoutClosedResult<TEvent>> ShowAsync<TContent, TEvent>(
            [NotNull] string title, [NotNull] TContent content, [CanBeNull] Thickness? margin = null, 
            FlyoutDisplayMode mode = FlyoutDisplayMode.ScrollableContent, bool stack = false, [CanBeNull] Action openCallback = null)
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
            Tuple<FlyoutContainer, Action> setup = SetupFlyoutContainer();

            // Prepare the flyout depending on the desired display mode
            switch (mode)
            {
                case FlyoutDisplayMode.ScrollableContent:
                    setup.Item1.SetupUI(title, content, margin);
                    break;
                case FlyoutDisplayMode.ActualHeight:
                    double width = CalculateExpectedWidth();
                    if (margin != null) content.Margin = margin.Value;
                    setup.Item1.SetupFixedUI(title, content, width);
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
                Child = setup.Item1
            };
            FlyoutDisplayInfo info = new FlyoutDisplayInfo(popup, mode);

            // Prepare the closed handler
            void CloseHandler(object s, object e)
            {
                // Results
                tcs.TrySetCanceled();
                popup.Child = null;
                popup.Closed -= CloseHandler;
                setup.Item2?.Invoke();
            }
            popup.Closed += CloseHandler;

            // Display and animate the popup
            DisableOpenFlyouts();
            PopupStack.Push(info);
            popup.SetVisualOpacity(0);
            popup.IsOpen = true;
            AdjustPopupSize(info, PopupStack.Count > 1);
            await popup.StartCompositionFadeSlideAnimationAsync(null, 1, TranslationAxis.Y, 20, 0, 250, null, null, EasingFunctionNames.CircleEaseOut);
            if (mode == FlyoutDisplayMode.ActualHeight) AdjustPopupSize(info, PopupStack.Count > 1, true);
            Semaphore.Release();
            openCallback?.Invoke();

            // Wait and return the right result
            return await tcs.Task.ContinueWith(t => t.Status == TaskStatus.RanToCompletion
                ? t.Result
                : FlyoutClosedResult<TEvent>.Closed);
        }

        #endregion

        #region Context menu APIs

        /// <summary>
        /// Shows a given content inside a popup with an animation and offset similar of an attached Flyout
        /// </summary>
        /// <param name="content">The control to show</param>
        /// <param name="target">The target element to try not to cover</param>
        /// <param name="tryCenter">Indicates whether or not to try to center the popup to the source control</param>
        public async void ShowCustomContextFlyout([NotNull] FrameworkElement content, [NotNull] FrameworkElement target, bool tryCenter = false)
        {
            // Calculate the target area for the context menu
            Point point = target.GetVisualCoordinates();
            Rect rect = new Rect(point, new Size(target.ActualWidth, target.ActualHeight));

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
            Offset CalculateOffset(Rect area)
            {
                // Calculate the final offset
                double
                    x = 0,
                    y = 0,
                    width = ResolutionHelper.CurrentWidth,
                    height = ResolutionHelper.CurrentHeight;
                if (content.Height <= area.Top - 8)
                {
                    y = area.Top - content.Height - 8;
                }
                else if (content.Height + 8 < height)
                {
                    y = 8;
                }
                if (content.Width < width - area.Left)
                {
                    x = area.Left;
                }
                else if (content.Width < width)
                {
                    x = width - content.Width;
                }

                // Shift the target position left if needed
                if (tryCenter)
                {
                    double
                        contentOffset = content.Width / 2,
                        half = area.Width / 2;
                    if (x > contentOffset - half) x -= contentOffset - half;
                    else if (x > 8) x = 8;
                }
                return new Offset(x, y);
            }

            // Create the popup to display
            Offset offset = CalculateOffset(rect);
            Popup popup = new Popup
            {
                IsLightDismissEnabled = false,
                VerticalOffset = offset.Y,
                HorizontalOffset = offset.X
            };

            // Create the grid with the content and its drop shadow
            Grid parent = new Grid();
            Grid grid = new Grid();
            parent.Children.Add(grid);

            // Build the shadow frame and insert the actual popup content
            foreach (Vector2 v in new[]
            {
                new Vector2((float)content.Width, 0),
                new Vector2(-(float)content.Width, 0),
                new Vector2(0, -(float)content.Height),
                new Vector2(0, (float)content.Height)
            })
            {
                // Setup the shadow
                Border border = new Border();
                grid.Children.Add(border);
                content.AttachVisualShadow(border, true, 
                    (float)content.Width + 1, (float)content.Height + 1,
                    Colors.Black, 1, -0.5f, -0.5f, 
                    (v.X >= 0 ? v.X : -v.X) < 0.1 
                    ? new Thickness(-4, 0, -4, 0) // Horizontal overstretch to fix UI glitch at the corners
                    : new Thickness(0, -4, 0, -4), v.X, v.Y);
            }
            grid.Children.Add(content);

            // Assign the popup content
            popup.Child = parent;
            grid.SetVisualOpacity(0);

            // Setup the hit target grid and its popup
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

            // Lights setup
            LightsSourceHelper.SetIsLightsContainer(parent, true);

            // Local functions
            void ClosePopups()
            {
                // The manual animation here is a workaround for a crash with the implicit hide composition animations
                _CloseContextMenu = null;
                hit.IsOpen = false;
                grid.IsHitTestVisible = false;
                grid.StartCompositionFadeSlideAnimation(1, 0, TranslationAxis.Y, 0, 8, 200, null, null, EasingFunctionNames.CircleEaseOut,
                    () => popup.IsOpen = false);
            }
            bool sizeHandled = true;
            void WindowSizeHandler(object s, WindowSizeChangedEventArgs e)
            {
                if (sizeHandled)
                {
                    sizeHandled = false;
                    Window.Current.SizeChanged -= WindowSizeHandler;
                    ClosePopups();
                }
            }

            // Setup the event handlers and display the popup
            popup.Closed += (s, e) =>
            {
                if (sizeHandled)
                {
                    sizeHandled = false;
                    Window.Current.SizeChanged -= WindowSizeHandler;
                }
                LightsSourceHelper.SetIsLightsContainer(parent, false);
            };
            _CloseContextMenu = ClosePopups;
            popup.IsOpen = true; // Open the context menu popup on top of the hit target
            hitGrid.Tapped += (_, e) => ClosePopups();
            Window.Current.SizeChanged += WindowSizeHandler;
            grid.StartCompositionFadeSlideAnimation(0, 1, TranslationAxis.Y, 20, 0, 200, null, null, EasingFunctionNames.CircleEaseOut);
            Semaphore.Release();

            // Adjust the offset after a delay, if needed
            Task.Delay(250).ContinueWith(t =>
            {
                // Check the updated target position
                point = target.GetVisualCoordinates();
                Rect delayedRect = new Rect(point, new Size(target.ActualWidth, target.ActualHeight));
                if (delayedRect.Left.EqualsWithDelta(rect.Left) &&
                    delayedRect.Top.EqualsWithDelta(rect.Top) ||
                    !popup.IsOpen) return;

                // Animate the popup to the new offset
                offset = CalculateOffset(delayedRect);
                XAMLTransformToolkit.PrepareStoryboard(
                    XAMLTransformToolkit.CreateDoubleAnimation(popup, "HorizontalOffset", null, offset.X, 250, EasingFunctionNames.CircleEaseOut, true),
                    XAMLTransformToolkit.CreateDoubleAnimation(popup, "VerticalOffset", null, offset.Y, 250, EasingFunctionNames.CircleEaseOut, true)).Begin();

            }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
        }

        #endregion
    }
}
