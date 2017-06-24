using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
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
        public FlyoutContainer()
        {
            Loaded += FlyoutContainer_Loaded;
            this.InitializeComponent();
            ConfirmButton.ManageControlPointerStates((p, value) => VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false));
        }

        // The in-app acrylic brush for the background of the popup
        private AttachedAnimatableCompositionEffect<Border> _LoadingAcrylic;

        // Initializes the acrylic effect
        private async void FlyoutContainer_Loaded(object sender, RoutedEventArgs e)
        {
            // Background effect
            await BlurBorder.AttachCompositionInAppCustomAcrylicEffectAsync(BlurBorder, 8, 800,
                Color.FromArgb(byte.MaxValue, 0x1B, 0x1B, 0x1B), 0.8f,
                Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"), disposeOnUnload: true);

            // Loading effect if needed
            if (_Content is IBusyWorkingContent)
            {
                _LoadingAcrylic = await LoadingBorder.AttachCompositionAnimatableInAppCustomAcrylicEffectAsync(LoadingBorder,
                    6, 0, false, Color.FromArgb(byte.MaxValue, 0x05, 0x05, 0x05), 0.2f,
                    LoadingCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"), disposeOnUnload: true);
                LoadingBorder.Visibility = Visibility.Collapsed;
            }
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
                worker.WorkingStateChanged += (_, e) =>
                {
                    // Blur effect
                    if (e)
                    {
                        LoadingBorder.SetVisualOpacity(0);
                        LoadingBorder.Visibility = Visibility.Visible;
                        LoadingBorder.StartCompositionFadeAnimation(null, 1, 200, null, EasingFunctionNames.Linear);
                    }
                    _LoadingAcrylic?.AnimateAsync(e ? FixedAnimationType.In : FixedAnimationType.Out, TimeSpan.FromMilliseconds(500));
                    if (!e)
                    {
                        Task.Delay(300).ContinueWith(t =>
                        {
                            LoadingBorder.StartCompositionFadeAnimation(null, 0, 200, null, EasingFunctionNames.Linear,
                                () => LoadingBorder.Visibility = Visibility.Collapsed);
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }

                    // Progress ring
                    if (e)
                    {
                        LoadingRing.SetVisualOpacity(0);
                        LoadingRing.IsActive = true;
                        LoadingRing.StartCompositionFadeAnimation(null, 1, 400, null, EasingFunctionNames.Linear);
                    }
                    else
                    {
                        LoadingRing.StartCompositionFadeAnimation(null, 0, 400, null, EasingFunctionNames.Linear, () => LoadingRing.IsActive = false);
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
                        InitialLoadingGrid.StartCompositionFadeAnimation(1, 0, 250, null, EasingFunctionNames.Linear);
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
    }
}
