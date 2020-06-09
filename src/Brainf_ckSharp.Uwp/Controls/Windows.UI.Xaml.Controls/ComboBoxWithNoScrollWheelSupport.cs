using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Brainf_ckSharp.Uwp.Controls.Windows.UI.Xaml.Controls
{
    /// <summary>
    /// A custom <see cref="ComboBox"/> that doesn't let users change the selected value with the scroll wheel
    /// </summary>
    public sealed class ComboBoxWithNoScrollWheelSupport : ComboBox
    {
        /// <inheritdoc/>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
