using System;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard.Controls
{
    public sealed partial class VirtualArrowKeyboardButton : UserControl
    {
        public VirtualArrowKeyboardButton()
        {
            this.InitializeComponent();
#if DEBUG
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;
#endif
            this.ManageControlPointerStates((type, value) =>
            {
                if (type != PointerDeviceType.Mouse) return;
                BackgroundBorder.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
            });
        }

        /// <summary>
        /// Gets or sets the icon to show on the button
        /// </summary>
        public String Icon
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
