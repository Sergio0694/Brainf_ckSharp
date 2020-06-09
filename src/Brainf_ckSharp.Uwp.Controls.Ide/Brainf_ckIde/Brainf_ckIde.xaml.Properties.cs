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
        /// Gets or sets the title of the tooltip displayed over a syntax error
        /// </summary>
        public string SyntaxErrorToolTipTitle
        {
            get => (string)GetValue(SyntaxErrorToolTipTitleProperty);
            set => SetValue(SyntaxErrorToolTipTitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the subtitle of the tooltip displayed over a syntax error
        /// </summary>
        public string SyntaxErrorToolTipSubtitle
        {
            get => (string)GetValue(SyntaxErrorToolTipSubtitleProperty);
            set => SetValue(SyntaxErrorToolTipSubtitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the syntax highlight theme to use
        /// </summary>
        public FrameworkElement ContextMenuSecondaryContent
        {
            get => CodeEditBox.ContextMenuSecondaryContent;
            set => CodeEditBox.ContextMenuSecondaryContent = value;
        }

        /// <summary>
        /// Gets or sets whether or not whitespace characters should be rendered
        /// </summary>
        public bool RenderWhitespaceCharacters
        {
            get => CodeEditBox.RenderWhitespaceCharacters;
            set => CodeEditBox.RenderWhitespaceCharacters = value;
        }

        /// <summary>
        /// Gets the dependency property for <see cref="SyntaxErrorToolTipTitle"/>.
        /// </summary>
        public static readonly DependencyProperty SyntaxErrorToolTipTitleProperty =
            DependencyProperty.Register(
                nameof(SyntaxErrorToolTipTitle),
                typeof(string),
                typeof(Brainf_ckIde),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the dependency property for <see cref="SyntaxErrorToolTipSubtitle"/>.
        /// </summary>
        public static readonly DependencyProperty SyntaxErrorToolTipSubtitleProperty =
            DependencyProperty.Register(
                nameof(SyntaxErrorToolTipSubtitle),
                typeof(string),
                typeof(Brainf_ckIde),
                new PropertyMetadata(string.Empty));

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
