using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls.CustomCommandBar
{
    public sealed class CustomCommandBarSeparator : AppBarSeparator, ICustomCommandBarPrimaryItem
    {
        /// <summary>
        /// Indicates whether the button should be visible by default or when the AutoHideCommandBar changes display mode
        /// </summary>
        public bool DefaultButton { get; set; }

        /// <summary>
        /// Raised when the value of the ExtraCondition parameter changes
        /// </summary>
        public event EventHandler<bool> ExtraConditionStateChanged;

        /// <summary>
        /// An additional condition that is required to be true in order for the control to be visible
        /// </summary>
        public bool ExtraCondition
        {
            get => GetValue(ExtraConditionProperty).To<bool>();
            set => SetValue(ExtraConditionProperty, value);
        }

        public static readonly DependencyProperty ExtraConditionProperty = DependencyProperty.Register(
            nameof(ExtraCondition), typeof(bool), typeof(CustomCommandBarSeparator), new PropertyMetadata(true, OnExtraConditionPropertyChanged));

        private static void OnExtraConditionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomCommandBarSeparator button = d.To<CustomCommandBarSeparator>();
            VisualStateManager.GoToState(button, "Normal", false);
            button.ExtraConditionStateChanged?.Invoke(d, e.NewValue.To<bool>());
        }

        /// <inheritdoc cref="IDisposable"/>
        public void Dispose() => ExtraConditionStateChanged = null;

        /// <inheritdoc cref="ICustomCommandBarPrimaryItem"/>
        public double DesiredOpacity { get; set; } = 1;

        /// <inheritdoc cref="ICustomCommandBarPrimaryItem"/>
        public FrameworkElement Control => this;
    }
}
