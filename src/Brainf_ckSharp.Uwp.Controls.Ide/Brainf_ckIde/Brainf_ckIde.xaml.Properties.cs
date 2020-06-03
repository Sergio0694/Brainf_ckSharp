using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Themes;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// Gets the text currently displayed
        /// </summary>
        public string Text => CodeEditBox.Text;

        /// <summary>
        /// Gets or sets the syntax highlight theme to use
        /// </summary>
        public FrameworkElement ContextMenuSecondaryContent
        {
            get => CodeEditBox.ContextMenuSecondaryContent;
            set => CodeEditBox.ContextMenuSecondaryContent = value;
        }

        /// <summary>
        /// Gets or sets the syntax highlight theme to use
        /// </summary>
        public Brainf_ckTheme SyntaxHighlightTheme
        {
            get => (Brainf_ckTheme)GetValue(SyntaxHighlightThemeProperty);
            set => SetValue(SyntaxHighlightThemeProperty, value);
        }

        /// <summary>
        /// Gets the dependency property for <see cref="SyntaxHighlightTheme"/>.
        /// </summary>
        public static readonly DependencyProperty SyntaxHighlightThemeProperty =
            DependencyProperty.Register(
                nameof(SyntaxHighlightTheme),
                typeof(Brainf_ckTheme),
                typeof(Brainf_ckIde),
                new PropertyMetadata(Brainf_ckThemes.VisualStudio));
    }
}
