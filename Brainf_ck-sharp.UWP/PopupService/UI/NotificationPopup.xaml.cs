using System;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.Resources;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Brushes;
using UICompositionAnimations.Enums;

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
        /// <param name="borderBrush">The brush for the light borders</param>
        /// <param name="hoverBrush">The brush for the light hover effect</param>
        public NotificationPopup([NotNull] String title, [NotNull] String icon, [NotNull] String content, NotificationType type,
            LightingBrush borderBrush, LightingBrush hoverBrush)
        {
            Loaded += NotificationPopup_Loaded;
            this.InitializeComponent();
            TitleBlock.Text = title;
            SymbolBlock.Text = icon;
            ContentBlock.Text = content;
            Type = type;
            LightBorder.BorderBrush = borderBrush;
            BackgroundBorder.Background = hoverBrush;
            CloseButton.ManageControlPointerStates((p, value) =>
            {
                if (p != PointerDeviceType.Mouse) return;
                BackgroundBorder.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
                LightBorder.StartXAMLTransformFadeAnimation(null, value ? 0 : 0.8, 200, null, EasingFunctionNames.Linear);
            });
            Unloaded += (s, e) =>
            {
                Win2DCanvas.RemoveFromVisualTree();
                Win2DCanvas = null;
            };
        }

        // Gets the current type of notification
        private readonly NotificationType Type;

        // Initializes the acrylic effect
        private async void NotificationPopup_Loaded(object sender, RoutedEventArgs e)
        {
            await AcrylicBorder.AttachCompositionInAppCustomAcrylicEffectAsync(AcrylicBorder, 8, 800,
                Type == NotificationType.Default 
                ? BrushResourcesManager.Instance.AccentBrush.Color
                : XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("OrangeWarningBrush").Color, 0.5f, null,
                Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"), disposeOnUnload: true);
        }

        // Signals a request to close the notification
        private void RequestCloseNotification() => Messenger.Default.Send(new NotificationCloseRequestMessage());
    }
}
