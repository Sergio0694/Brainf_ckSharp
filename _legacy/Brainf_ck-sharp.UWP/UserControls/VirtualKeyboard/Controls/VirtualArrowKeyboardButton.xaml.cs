using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.VirtualKeyboard.Controls
{
    public sealed partial class VirtualArrowKeyboardButton : UserControl
    {
        public VirtualArrowKeyboardButton()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the icon to show on the button
        /// </summary>
        public string Icon
        {
            get => IconBlock.Text;
            set => IconBlock.Text = value;
        }

        /// <summary>
        /// Raised whenever the button in the current control is clicked by the user
        /// </summary>
        public event EventHandler Click;

        // Raises the Click event
        private void Button_Click(object sender, RoutedEventArgs e) => Click?.Invoke(this, EventArgs.Empty);
    }
}
