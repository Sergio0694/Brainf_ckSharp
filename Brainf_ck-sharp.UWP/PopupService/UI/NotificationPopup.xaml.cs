using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.Resources;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects.Base;

namespace Brainf_ck_sharp_UWP.PopupService.UI
{
    public sealed partial class NotificationPopup : UserControl
    {
        /// <summary>
        /// Creates a new notification to show to the user
        /// </summary>
        /// <param name="title">The title of the notification</param>
        /// <param name="icon">The icon to show next to the title</param>
        /// <param name="content">The text to show inside the notification</param>
        /// <param name="type">Indicates the kind of notification to show</param>
        public NotificationPopup([NotNull] String title, [NotNull] String icon, [NotNull] String content, NotificationType type)
        {
            Loaded += NotificationPopup_Loaded;
            SizeChanged += (_, e) => _AcrylicEffect?.AdjustSize(e.NewSize.Width, e.NewSize.Height);
            this.InitializeComponent();
            TitleBlock.Text = title;
            SymbolBlock.Text = icon;
            ContentBlock.Text = content;
            Type = type;
        }

        // Gets the current type of notification
        private readonly NotificationType Type;

        // The background in-app acrylic effect for the notification background
        private AttachedStaticCompositionEffect<Border> _AcrylicEffect;

        // Initializes the acrylic effect
        private async void NotificationPopup_Loaded(object sender, RoutedEventArgs e)
        {
            _AcrylicEffect = await AcrylicBorder.GetAttachedInAppSemiAcrylicEffectAsync(AcrylicBorder, 8, 800,
                Type == NotificationType.Default 
                ? BrushResourcesManager.Instance.AccentBrush.Color
                : XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("OrangeWarningBrush").Color, 0.5f,
                Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
        }

        // Signals a request to close the notification
        private void RequestCloseNotification() => Messenger.Default.Send(new NotificationCloseRequestMessage());
    }
}
