using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects.Base;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
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
        }

        private AttachedStaticCompositionEffect<Border> _BackgroundAcrylic;

        private async void FlyoutContainer_Loaded(object sender, RoutedEventArgs e)
        {
            _BackgroundAcrylic = await BlurBorder.GetAttachedInAppSemiAcrylicEffectAsync(BlurBorder, 8, 800,
                Color.FromArgb(byte.MaxValue, 40, 40, 40), 0.6f,
                Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
        }

        /// <summary>
        /// Sends a message to request the flyout to be closed
        /// </summary>
        public void RequestClose() => Messenger.Default.Send(new FlyoutCloseRequestMessage());

        /// <summary>
        /// Prepares the container to show a given element
        /// </summary>
        /// <param name="title">The title for the container</param>
        /// <param name="content">The element to host in the container</param>
        public void SetupUI([NotNull] String title, FrameworkElement content)
        {
            TitleBlock.Text = title;
            Grid.SetRow(content, 1);
            RootGrid.Children.Add(content);
        }
    }
}
