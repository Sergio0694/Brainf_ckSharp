using Windows.Foundation;
using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Controls.Ide.EventArgs;
using Brainf_ckSharp.Uwp.Themes;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// Raised whenever the <see cref="Text"/> property changes
        /// </summary>
        public event TypedEventHandler<Brainf_ckIde, PlainTextChangedEventArgs>? TextChanged;

        /// <summary>
        /// Gets the text currently displayed
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            private set => SetValue(TextProperty, value);
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
        /// Gets the dependency property for <see cref="Text"/>.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(Brainf_ckIde),
                new PropertyMetadata("\r"));

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
