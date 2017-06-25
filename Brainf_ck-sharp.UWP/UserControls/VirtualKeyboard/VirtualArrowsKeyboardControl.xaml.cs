using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Messages.Actions;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations.Behaviours;

namespace Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard
{
    public sealed partial class VirtualArrowsKeyboardControl : UserControl
    {
        public VirtualArrowsKeyboardControl()
        {
            Loaded += VirtualArrowsKeyboardControl_Loaded;
            this.InitializeComponent();
            Unloaded += (s, e) =>
            {
                Win2DCanvas.RemoveFromVisualTree();
                Win2DCanvas = null;
            };
        }

        // Initialize the effect brush
        private async void VirtualArrowsKeyboardControl_Loaded(object sender, RoutedEventArgs e)
        {
            await EffectBorder.AttachCompositionInAppCustomAcrylicEffectAsync(EffectBorder, 6, 600,
                Color.FromArgb(byte.MaxValue, 0x1A, 0x1A, 0x1A), 0.6f, null,
                Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"), disposeOnUnload: true);
        }

        #region Key messages

        public void SendKeyUpMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Up));

        public void SendKeyLeftMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Left));

        public void SendKeyDownMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Down));

        public void SendKeyRightMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Right));

        #endregion
    }
}
