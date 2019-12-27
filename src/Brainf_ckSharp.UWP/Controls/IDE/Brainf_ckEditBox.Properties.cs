using Windows.UI.Xaml;

namespace Brainf_ckSharp.UWP.Controls.IDE
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// Gets or sets whether or not to automatically indent brackets and parentheses
        /// </summary>
        public bool IsAutomaticBracketsIndentationEnabled
        {
            get => (bool)GetValue(IsAutomaticBracketsIndentationEnabledProperty);
            set => SetValue(IsAutomaticBracketsIndentationEnabledProperty, value);
        }

        /// <summary>
        /// Gets the dependency property for <see cref="IsAutomaticBracketsIndentationEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsAutomaticBracketsIndentationEnabledProperty =
            DependencyProperty.Register(
                nameof(IsAutomaticBracketsIndentationEnabled),
                typeof(bool),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata(default(bool)));
    }
}
