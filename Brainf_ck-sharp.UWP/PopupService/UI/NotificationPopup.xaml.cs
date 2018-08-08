using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Brushes;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers.PointerEvents;

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
        public NotificationPopup([NotNull] string title, [NotNull] string icon, [NotNull] string content, NotificationType type)
        {
            this.InitializeComponent();
            Root.Background = XAMLResourcesHelper.GetResourceValue<CustomAcrylicBrush>(
                type == NotificationType.Default ? "NotificationAcrylicBrush" : "NotificationErrorAcrylicBrush");
            TitleBlock.Text = title;
            SymbolBlock.Text = icon;
            ContentBlock.Text = content;
            CloseButton.ManageLightsPointerStates(value =>
            {
                BackgroundBorder.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
                LightBorder.StartXAMLTransformFadeAnimation(null, value ? 0 : 0.2, 200, null, EasingFunctionNames.Linear);
            });
        }

        // Signals a request to close the notification
        private void RequestCloseNotification() => Messenger.Default.Send(new NotificationCloseRequestMessage());
    }
}
