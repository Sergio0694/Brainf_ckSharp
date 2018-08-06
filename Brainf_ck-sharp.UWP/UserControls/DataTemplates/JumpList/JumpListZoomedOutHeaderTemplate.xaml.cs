using System;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers.PointerEvents;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.JumpList
{
    public sealed partial class JumpListZoomedOutHeaderTemplate : UserControl
    {
        public JumpListZoomedOutHeaderTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((pointer, value) =>
            {
                // Visual states
                VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false);

                // Lights
                if (pointer != PointerDeviceType.Mouse) return;
                LightBackground.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
            });
        }

        /// <summary>
        /// Gets or sets the title to display in the control
        /// </summary>
        public string Title
        {
            get => (String)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(String), typeof(JumpListZoomedOutHeaderTemplate), new PropertyMetadata(default(String), OnTitlePropertyChanged));

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<JumpListZoomedOutHeaderTemplate>().TitleBlock.Text = e.NewValue.To<string>() ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the description of the control
        /// </summary>
        public string Description
        {
            get => (String)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description), typeof(String), typeof(JumpListZoomedOutHeaderTemplate), 
            new PropertyMetadata(default(String), OnDescriptionPropertyChanged));

        private static void OnDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<JumpListZoomedOutHeaderTemplate>().InfoBlock.Text = e.NewValue.To<string>() ?? string.Empty;
        }
    }
}
