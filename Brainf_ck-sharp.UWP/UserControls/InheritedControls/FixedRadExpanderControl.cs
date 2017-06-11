using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A custom <see cref="RadExpanderControl"/> that can only be expanded when tapping on the right indicator
    /// </summary>
    public class FixedRadExpanderControl : RadExpanderControl
    {
        // Ignores the key events to prevent the control to expand and collapse by accident
        protected override void OnKeyDown(KeyRoutedEventArgs e) { }

        // Only propagates the event if the tap originated inside the indicator area
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (e.GetPosition(this).X < ActualWidth - ExpanderHitTargetAreaWidth)
            {
                e.Handled = true;
            }
            base.OnTapped(e);
        }

        /// <summary>
        /// Gets or sets the width of the right indicator area where the expander will react to tapped events
        /// </summary>
        public double ExpanderHitTargetAreaWidth
        {
            get => (double)GetValue(ExpanderHitTargetAreaWidthProperty);
            set => SetValue(ExpanderHitTargetAreaWidthProperty, value);
        }

        public static readonly DependencyProperty ExpanderHitTargetAreaWidthProperty = DependencyProperty.Register(
            nameof(ExpanderHitTargetAreaWidth), typeof(double), typeof(FixedRadExpanderControl), new PropertyMetadata(default(double)));
    }
}
