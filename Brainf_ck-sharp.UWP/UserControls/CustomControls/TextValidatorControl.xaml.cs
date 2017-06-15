using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.UserControls.CustomControls
{
    public sealed partial class TextValidatorControl : UserControl
    {
        public TextValidatorControl()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "Invalid", false);
        }

        /// <summary>
        /// Gets the current state of the validator control
        /// </summary>
        public bool Validated
        {
            get { return (bool)GetValue(ValidatedProperty); }
            set { SetValue(ValidatedProperty, value); }
        }

        public static readonly DependencyProperty ValidatedProperty = DependencyProperty.Register(
            nameof(Validated), typeof(bool), typeof(TextValidatorControl), new PropertyMetadata(DependencyProperty.UnsetValue, OnValidatedPropertyChanged));

        private static void OnValidatedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(d.To<TextValidatorControl>(), e.NewValue.To<bool>() ? "Valid" : "Invalid", false);
        }

        /// <summary>
        /// Gets or sets the text displayed next to the status icon
        /// </summary>
        public String DisplayedInfo
        {
            get { return GetValue(DisplayedInfoProperty).To<String>(); }
            set { SetValue(DisplayedInfoProperty, value); }
        }

        public static readonly DependencyProperty DisplayedInfoProperty = DependencyProperty.Register(
            nameof(DisplayedInfo), typeof(String), typeof(TextValidatorControl), new PropertyMetadata(DependencyProperty.UnsetValue, OnDisplayedInfoPropertyChanged));

        private static void OnDisplayedInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<TextValidatorControl>().ValidatedTextBlock.Text = e.NewValue.To<String>() ?? String.Empty;
        }
    }
}
