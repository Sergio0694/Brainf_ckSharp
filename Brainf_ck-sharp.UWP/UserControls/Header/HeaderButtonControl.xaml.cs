using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.UserControls.Header
{
    public sealed partial class HeaderButtonControl : UserControl
    {
        public HeaderButtonControl()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "Default", false);
            this.ManageControlPointerStates((_, value) =>
            {
                LightBorder.StartXAMLTransformFadeAnimation(null, value ? 0 : 1, 200, null, EasingFunctionNames.Linear);
            });
        }

        /// <summary>
        /// Gets or sets the icon to display in the control (must have the format &#x[0-F]{4];)
        /// </summary>
        public String IconText
        {
            get => IconBlock.Text;
            set => IconBlock.Text = value;
        }

        /// <summary>
        /// gets or sets the title of the button
        /// </summary>
        public String Title
        {
            get => TitleBlock.Text;
            set => TitleBlock.Text = value;
        }

        /// <summary>
        /// Raised whenever the control is selected
        /// </summary>
        public event EventHandler HeaderSelected;

        /// <summary>
        /// Gets or sets whether or not the button is currently selected
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(HeaderButtonControl), new PropertyMetadata(default(bool), OnIsSelectedPropertyChanged));

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Raise the event if needed
            HeaderButtonControl @this = d.To<HeaderButtonControl>();
            if (e.NewValue is bool value && value)
            {
                @this.HeaderSelected?.Invoke(d, EventArgs.Empty);
                VisualStateManager.GoToState(@this, "Selected", false);
            }
            else VisualStateManager.GoToState(@this, "Default", false);
        }

        /// <summary>
        /// Tries to set the current control as selected
        /// </summary>
        public void TrySelect() => IsSelected = true;

        private void Header_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            TrySelect();
        }
    }
}
