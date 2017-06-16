﻿using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.FlyoutService.Interfaces;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects.Base;

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
            SizeChanged += (_, e) => _BackgroundAcrylic?.AdjustSize((float)e.NewSize.Width, (float)e.NewSize.Height);
            this.InitializeComponent();
            ConfirmButton.ManageControlPointerStates((p, value) => VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false));
        }

        // The in-app acrylic brush for the background of the popup
        private AttachedStaticCompositionEffect<Border> _BackgroundAcrylic;

        // Initializes the acrylic effect
        private async void FlyoutContainer_Loaded(object sender, RoutedEventArgs e)
        {
            _BackgroundAcrylic = await BlurBorder.GetAttachedInAppSemiAcrylicEffectAsync(BlurBorder, 8, 800,
                Color.FromArgb(byte.MaxValue, 0x1B, 0x1B, 0x1B), 0.8f,
                Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
        }

        /// <summary>
        /// Sends a message to request the flyout to be closed
        /// </summary>
        public void RequestClose() => Messenger.Default.Send(new FlyoutCloseRequestMessage());

        /// <summary>
        /// Gets the current display mode for the flyout host
        /// </summary>
        public FlyoutDisplayMode DisplayMode { get; private set; }

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
            DisplayMode = FlyoutDisplayMode.ScrollableContent;
            TitleBlock.Text = title;
            Grid.SetRow(content, 1);
            content.Margin = margin ?? new Thickness(12, 0, 0, 0);
            RootGrid.Children.Add(content);
            InterfacesSetup(content);
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
            DisplayMode = FlyoutDisplayMode.ActualHeight;
            TitleBlock.Text = title;
            ContentScroller.Content = content;
            ContentScroller.Visibility = Visibility.Visible;
            InterfacesSetup(content);
        }

        /// <summary>
        /// Adjusts the UI according to the interfaces implemented by the current content
        /// </summary>
        /// <param name="element"></param>
        private void InterfacesSetup(FrameworkElement element)
        {
            // Show the button and bind its enabled status
            if (element is IValidableDialog validable)
            {
                ConfirmButton.Visibility = Visibility.Visible;
                validable.ValidStateChanged += (s, e) => ConfirmButton.IsEnabled = e;
            }
        }

        /// <summary>
        /// Calculates the desired size for the flyout container and its content, when using a given width
        /// </summary>
        public Size CalculateDesiredSize()
        {
            if (DisplayMode != FlyoutDisplayMode.ActualHeight) throw new InvalidOperationException("Invalid display mode");
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