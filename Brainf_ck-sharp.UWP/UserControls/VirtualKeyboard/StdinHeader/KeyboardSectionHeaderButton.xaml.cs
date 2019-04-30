using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard.StdinHeader
{
    public sealed partial class KeyboardSectionHeaderButton : UserControl
    {
        public KeyboardSectionHeaderButton()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "Default", false);
        }

        /// <inheritdoc cref="UserControl"/>
        public new bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                if (IsEnabled != value)
                {
                    VisualStateManager.GoToState(this, value ? "Default" : "Disabled", false);
                    base.IsEnabled = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon to display in the control (must have the format &#x[0-F]{4];)
        /// </summary>
        public string IconText
        {
            get => IconBlock.Text;
            set => IconBlock.Text = value;
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
            nameof(IsSelected), typeof(bool), typeof(KeyboardSectionHeaderButton), new PropertyMetadata(default(bool), OnIsSelectedPropertyChanged));

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Raise the event if needed
            KeyboardSectionHeaderButton @this = d.To<KeyboardSectionHeaderButton>();
            if (e.NewValue is bool value && value)
            {
                @this.HeaderSelected?.Invoke(d, EventArgs.Empty);
                VisualStateManager.GoToState(@this, "Selected", false);
            }
            else if (@this.IsEnabled) VisualStateManager.GoToState(@this, "Default", false);
        }

        /// <summary>
        /// Tries to set the current control as selected
        /// </summary>
        public void TrySelect() => IsSelected = true;
    }
}
