using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo.Donations
{
    public sealed partial class DevSupportOptionEntryControl : UserControl
    {
        public DevSupportOptionEntryControl()
        {
            this.InitializeComponent();
            this.ManageLightsPointerStates(value =>
            {
                // Lights
                LightBackground.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
            });
        }

        /// <summary>
        /// Gets or sets the image to display inside the control
        /// </summary>
        public ImageSource ImageData
        {
            get => EntryImage.Source;
            set => EntryImage.Source = value;
        }

        /// <summary>
        /// Gets or sets the title of the control
        /// </summary>
        public String PickerTitle
        {
            get => this.TitleBlock.Text;
            set => this.TitleBlock.Text = value;
        }

        /// <summary>
        /// Gets or sets the description of the control
        /// </summary>
        public String PickerDescription
        {
            get => this.DescriptionBlock.Text;
            set => this.DescriptionBlock.Text = value;
        }
    }
}
