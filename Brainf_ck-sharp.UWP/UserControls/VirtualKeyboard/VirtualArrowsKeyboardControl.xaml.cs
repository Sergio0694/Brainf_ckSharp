using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Messages.Actions;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects.Base;

namespace Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard
{
    public sealed partial class VirtualArrowsKeyboardControl : UserControl
    {
        public VirtualArrowsKeyboardControl()
        {
            SizeChanged += (_, e) => _BorderEffect?.AdjustSize(e.NewSize.Width, e.NewSize.Height);
            Loaded += VirtualArrowsKeyboardControl_Loaded;
            this.InitializeComponent();
        }

        // In-app acrylic brush effect
        private AttachedStaticCompositionEffect<Border> _BorderEffect;

        // Initialize the effect brush
        private async void VirtualArrowsKeyboardControl_Loaded(object sender, RoutedEventArgs e)
        {
            _BorderEffect = await EffectBorder.GetAttachedInAppSemiAcrylicEffectAsync(EffectBorder, 8, 800,
                Color.FromArgb(byte.MaxValue, 30, 30, 30), 0.6f,
                Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
        }

        #region Key messages

        public void SendKeyUpMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Up));

        public void SendKeyLeftMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Left));

        public void SendKeyDownMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Down));

        public void SendKeyRightMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Right));

        #endregion
    }
}
