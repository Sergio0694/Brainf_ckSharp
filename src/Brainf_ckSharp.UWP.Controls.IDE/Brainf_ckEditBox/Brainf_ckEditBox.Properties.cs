using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Themes;

namespace Brainf_ckSharp.Uwp.Controls.Ide
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
        /// Gets or sets the syntax highlight theme to use
        /// </summary>
        public ThemeInfo SyntaxHighlightTheme
        {
            get => (ThemeInfo)GetValue(SyntaxHighlightThemeProperty);
            set => SetValue(SyntaxHighlightThemeProperty, value);
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

        /// <summary>
        /// Gets the dependency property for <see cref="SyntaxHighlightTheme"/>.
        /// </summary>
        public static readonly DependencyProperty SyntaxHighlightThemeProperty =
            DependencyProperty.Register(
                nameof(SyntaxHighlightTheme),
                typeof(ThemeInfo),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata(Themes.Themes.VisualStudio));
    }
}
