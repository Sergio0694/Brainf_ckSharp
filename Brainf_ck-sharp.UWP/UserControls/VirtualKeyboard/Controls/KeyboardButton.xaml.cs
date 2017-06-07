using System;
using Windows.UI.Xaml.Controls;

namespace Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard.Controls
{
    public sealed partial class KeyboardButton : UserControl
    {
        public KeyboardButton()
        {
            this.InitializeComponent();
        }

        public String Text
        {
            get => OperatorBlock.Text;
            set => OperatorBlock.Text = value;
        }

        public String Description
        {
            get => InfoBlock.Text;
            set => InfoBlock.Text = value;
        }
    }
}
