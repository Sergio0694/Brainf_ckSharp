using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard.Controls
{
    /// <summary>
    /// A keyboard button with a Brainf_ck operator and its info
    /// </summary>
    public sealed partial class KeyboardButton : UserControl
    {
        public KeyboardButton()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((_, value) =>
            {
                LightBorder.StartXAMLTransformFadeAnimation(null, value ? 0 : 1, 200, null, EasingFunctionNames.Linear);
            });
        }

        /// <summary>
        /// Gets or sets the operator to show on the button
        /// </summary>
        public String Text
        {
            get => OperatorBlock.Text;
            set => OperatorBlock.Text = value;
        }

        /// <summary>
        /// Gets or sets the description of the button
        /// </summary>
        public String Description
        {
            get => InfoBlock.Text;
            set => InfoBlock.Text = value;
        }

        /// <summary>
        /// Raised whenever the user clicks on the button instance
        /// </summary>
        public event RoutedEventHandler Click;

        private void Button_Clicked(object sender, RoutedEventArgs e) => Click?.Invoke(this, e);
    }
}
