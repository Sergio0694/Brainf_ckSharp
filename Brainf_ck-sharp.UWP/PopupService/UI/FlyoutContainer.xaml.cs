using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp;
using UICompositionAnimations;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects;
using UICompositionAnimations.Behaviours.Misc;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.PopupService.UI
{
    /// <summary>
    /// A container to display different popups across the app
    /// </summary>
    public sealed partial class FlyoutContainer : UserControl
    {
        /// <summary>
        /// Creates a new container that will load the desired background effect when displayed
        /// </summary>
        /// <param name="tint">The optional tint color to use</param>
        /// <param name="tintMix">The optional mix value for the background tint color</param>
        public FlyoutContainer(Color? tint, float? tintMix)
        {
            TintColor = tint;
            TintMix = tintMix;
            Unloaded += FlyoutContainer_Unloaded;
            Loaded += FlyoutContainer_Loaded;
            this.InitializeComponent();
            ConfirmButton.ManageControlPointerStates((p, value) =>
            {
                VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false);
                if (p != PointerDeviceType.Mouse) return;
                ConfirmBackgroundBorder.StartXAMLTransformFadeAnimation(null, value ? 0.8 : 0, 200, null, EasingFunctionNames.Linear);
            });
            CloseButton.ManageControlPointerStates((type, value) =>
            {
                if (type != PointerDeviceType.Mouse) return;
                BackgroundBorder.StartXAMLTransformFadeAnimation(null, value ? 0.8 : 0, 200, null, EasingFunctionNames.Linear);
                LightBorder.StartXAMLTransformFadeAnimation(null, value ? 0 : 0.8, 200, null, EasingFunctionNames.Linear);
            });
        }

        // Resources cleanup
        private void FlyoutContainer_Unloaded(object sender, RoutedEventArgs e)
        {
            LoadingCanvas.RemoveFromVisualTree();
            LoadingCanvas = null;
            Win2DCanvas.RemoveFromVisualTree();
            Win2DCanvas = null;
            _DetachContent?.Invoke();
            _DetachContent = null;
            _Content = null;
        }

        // The in-app acrylic brush for the background of the popup
        private AttachedAnimatableCompositionEffect<Border> _LoadingAcrylic;

        // The optional custom tint color for the popup background
        private readonly Color? TintColor;

        // The optional custom tint mix level for the popup background
        private readonly float? TintMix;

        // Initializes the acrylic effect
        private async void FlyoutContainer_Loaded(object sender, RoutedEventArgs e)
        {
            // Background effect
            await BlurBorder.AttachCompositionInAppCustomAcrylicEffectAsync(BlurBorder, 8, 800,
                TintColor ?? Color.FromArgb(byte.MaxValue, 0x1B, 0x1B, 0x1B), TintMix ?? 0.8f, null,
                Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"), disposeOnUnload: true);
        }

        /// <summary>
        /// Sends a message to request the flyout to be closed
        /// </summary>
        public void RequestClose() => Messenger.Default.Send(new FlyoutCloseRequestMessage());

        /// <summary>
        /// Gets the current display mode for the flyout host
        /// </summary>
        private FlyoutDisplayMode _DisplayMode;

        // The content currently hosted by the container
        private FrameworkElement _Content;

        // An action that removes the custom content once no longer needed
        [CanBeNull]
        private Action _DetachContent;

        /// <summary>
        /// Prepares the UI for a text content with a confirm button
        /// </summary>
        /// <param name="confirm">The text of the button</param>
        /// <param name="color">The optional background color for the confirm button</param>
        public void SetupUI([NotNull] String confirm, [CanBeNull] Color? color)
        {
            ConfirmBlock.Text = confirm;
            if (color != null)
            {
                DefaultSetter.Value = new SolidColorBrush(color.Value);
                HslColor hsl = color.Value.ToHsl();
                hsl.L = hsl.L + 0.1 > 1 ? 1 : hsl.L + 0.1;
                HighlightSetter.Value = new SolidColorBrush(Microsoft.Toolkit.Uwp.ColorHelper.FromHsl(hsl.H, hsl.S, hsl.L, hsl.A));
                ButtonContentGrid.Background = new SolidColorBrush(color.Value);
            }
            ConfirmButton.IsEnabled = true;
            ConfirmButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Prepares the container to show a given element
        /// </summary>
        /// <param name="title">The title for the container</param>
        /// <param name="content">The element to host in the container</param>
        /// <param name="margin">The optional margins to set to the content of the popup to show</param>
        public void SetupUI([NotNull] String title, FrameworkElement content, Thickness? margin)
        {
            _Content = content;
            _DisplayMode = FlyoutDisplayMode.ScrollableContent;
            TitleBlock.Text = title;
            Grid.SetRow(content, 1);
            content.Margin = margin ?? new Thickness(12, 0, 0, 0);
            RootGrid.Children.Add(content);
            _DetachContent = () => RootGrid.Children.Remove(content);
            InterfacesSetup();
        }

        /// <summary>
        /// Prepares the contains to host a content that is fixed and doesn't contain its own scroller
        /// </summary>
        /// <param name="title">The title for the container</param>
        /// <param name="content">The element to host in the container</param>
        /// <param name="width">The planned width for the rendered container</param>
        public void SetupFixedUI([NotNull] String title, FrameworkElement content, double width)
        {
            _Content = content;
            _DisplayMode = FlyoutDisplayMode.ActualHeight;
            TitleBlock.Text = title;
            ContentScroller.Content = content;
            ContentScroller.Visibility = Visibility.Visible;
            _DetachContent = () => ContentScroller.Content = content;
            InterfacesSetup();
        }

        /// <summary>
        /// Adjusts the UI according to the interfaces implemented by the current content
        /// </summary>
        private void InterfacesSetup()
        {
            // Show the button and bind its enabled status
            if (_Content is IValidableDialog validable)
            {
                ConfirmButton.Visibility = Visibility.Visible;
                validable.ValidStateChanged += (s, e) => ConfirmButton.IsEnabled = e;
            }
            
            // Loading content
            if (_Content is IBusyWorkingContent worker)
            {
                worker.WorkingStateChanged += async (_, e) =>
                {
                    // Load the effect if needed
                    if (_LoadingAcrylic == null)
                    {
                        _LoadingAcrylic = await LoadingBorder.AttachCompositionAnimatableInAppCustomAcrylicEffectAsync(LoadingBorder,
                            8, 0, false, Colors.Black, 0.2f,
                            LoadingCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"), disposeOnUnload: true);
                    }

                    // Fade in
                    if (e)
                    {
                        // Blur effect
                        LoadingBorder.SetVisualOpacity(0);
                        LoadingGrid.Visibility = Visibility.Visible;
                        LoadingBorder.StartCompositionFadeAnimation(null, 1, 200, null, EasingFunctionNames.Linear);

                        // Loading ring
                        LoadingRing.SetVisualOpacity(0);
                        LoadingRing.IsActive = true;
                        LoadingRing.StartCompositionFadeAnimation(null, 1, 400, null, EasingFunctionNames.Linear);
                    }
                    _LoadingAcrylic?.AnimateAsync(e ? FixedAnimationType.In : FixedAnimationType.Out, TimeSpan.FromMilliseconds(500));

                    // Fade out
                    if (!e)
                    {
                        LoadingRing.StartCompositionFadeAnimation(null, 0, 400, null, EasingFunctionNames.Linear, () => LoadingRing.IsActive = false);
                        LoadingBorder.StartCompositionFadeAnimation(null, 0, 200, 300, EasingFunctionNames.Linear,
                            () => LoadingGrid.Visibility = Visibility.Collapsed);
                    }
                };
            }

            // Async loaded content
            if (_Content is IAsyncLoadedContent loading)
            {
                // Setup the loading UI if needed
                if (loading.LoadingPending)
                {
                    InitialLoadingGrid.Visibility = Visibility.Visible;
                    loading.LoadingCompleted += (s, e) =>
                    {
                        InitialProgressRing.StartCompositionScaleAnimation(1, 1.1f, 250, null, EasingFunctionNames.Linear);
                        InitialLoadingGrid.StartCompositionFadeAnimation(1, 0, 250, null, EasingFunctionNames.Linear, 
                            () => InitialLoadingGrid.Visibility = Visibility.Collapsed);
                    };
                }
            }
        }

        /// <summary>
        /// Calculates the desired size for the flyout container and its content, when using a given width
        /// </summary>
        public Size CalculateDesiredSize()
        {
            if (_DisplayMode != FlyoutDisplayMode.ActualHeight) throw new InvalidOperationException("Invalid display mode");
            _Content.Measure(new Size(Width - 2, double.PositiveInfinity)); // Consider the side 1-px borders
            double buttonHeight = ConfirmButton.Visibility == Visibility.Visible ? 60 : 0;
            return new Size(Width, 52 + _Content.DesiredSize.Height + buttonHeight + 2);
        }

        /// <summary>
        /// Gets whether or not the flyout has been manually confirmed by the user by tapping the buttom
        /// </summary>
        public bool Confirmed { get; private set; }

        // Sets the confirm status and closes the popup
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            RequestClose();
        }

        // Closes the popup without confirm
        private void CloseButton_Click(object sender, RoutedEventArgs e) => RequestClose();
    }
}
