using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Messages.Actions;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard
{
    public sealed partial class VirtualArrowsKeyboardControl : UserControl
    {
        public VirtualArrowsKeyboardControl()
        {
            this.InitializeComponent();
        }

        #region Key messages

        public void SendKeyUpMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Up));

        public void SendKeyLeftMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Left));

        public void SendKeyDownMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Down));

        public void SendKeyRightMessage() => Messenger.Default.Send(new VirtualArrowKeyPressedMessage(VirtualArrow.Right));

        #endregion
    }
}
