using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.InheritedControls.CustomCommandBar
{
    /// <summary>
    /// A custom CommandBarButton to be put inside the AutoHideCommandBar
    /// </summary>
    public class CustomCommandBarButton : AppBarButton, ICustomCommandBarPrimaryItem
    {
        /// <summary>
        /// Indicates whether the button should be visible by default or when the AutoHideCommandBar changes display mode
        /// </summary>
        public bool DefaultButton { get; set; }

        /// <summary>
        /// Raised when the value of the ExtraCondition parameter changes
        /// </summary>
        public event EventHandler<bool> ExtraConditionStateChanged;

        /// <inheritdoc cref="IDisposable"/>
        public void Dispose() => ExtraConditionStateChanged = null;

        /// <summary>
        /// An additional condition that is required to be true in order for the control to be visible
        /// </summary>
        public bool ExtraCondition
        {
            get => GetValue(ExtraConditionProperty).To<bool>();
            set => SetValue(ExtraConditionProperty, value);
        }

        public static readonly DependencyProperty ExtraConditionProperty = DependencyProperty.Register(
            nameof(ExtraCondition), typeof(bool), typeof(CustomCommandBarButton), new PropertyMetadata(true, OnExtraConditionPropertyChanged));

        private static void OnExtraConditionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomCommandBarButton button = d.To<CustomCommandBarButton>();
            VisualStateManager.GoToState(button, "Normal", false);
            button.ExtraConditionStateChanged?.Invoke(d, e.NewValue.To<bool>());
        }

        /// <inheritdoc cref="ICustomCommandBarPrimaryItem"/>
        public double DesiredOpacity { get; set; } = 1;

        /// <inheritdoc cref="ICustomCommandBarPrimaryItem"/>
        public FrameworkElement Control => this;
    }
}
