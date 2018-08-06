using System;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers.PointerEvents;

namespace Brainf_ck_sharp_UWP.UserControls.CustomControls
{
    public sealed partial class IDESessionBreakpointOptionControl : UserControl
    {
        public IDESessionBreakpointOptionControl()
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
            nameof(Title), typeof(String), typeof(IDESessionBreakpointOptionControl), new PropertyMetadata(default(String), OnTitlePropertyChanged));

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<IDESessionBreakpointOptionControl>().TitleBlock.Text = e.NewValue.To<string>() ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the content to display as the icon of the control
        /// </summary>
        public FrameworkElement IconContent
        {
            get => (FrameworkElement)GetValue(IconContentProperty);
            set => SetValue(IconContentProperty, value);
        }

        public static readonly DependencyProperty IconContentProperty = DependencyProperty.Register(
            nameof(IconContent), typeof(FrameworkElement), typeof(IDESessionBreakpointOptionControl), 
            new PropertyMetadata(default(FrameworkElement), OnIconContentPropertyChanged));

        private static void OnIconContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IDESessionBreakpointOptionControl @this = d.To<IDESessionBreakpointOptionControl>();
            @this.IconHost.Children.Clear();
            if (e.NewValue is FrameworkElement content)
            {
                @this.IconHost.Children.Add(content);
            }
        }
    }
}
