using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Themes;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide;

public sealed partial class Brainf_ckIde
{
    /// <summary>
    /// Gets the text currently displayed
    /// </summary>
    public string Text => CodeEditBox.Text;

    private double _HeaderSpacing;

    /// <summary>
    /// Gets or sets the spacing of the top header
    /// </summary>
    public double HeaderSpacing
    {
        get => _HeaderSpacing;
        set
        {
            _HeaderSpacing = value;

            IdeOverlaysCanvasTransform.Y = value + 10;
            LineBlockTransform.Y = value + 8;
            CodeEditBox.Padding = new Thickness(4, value + 8, 8, FooterSpacing + 8);
            CodeEditBox.VerticalScrollBarMargin = new Thickness(0, value, 0, FooterSpacing);
        }
    }

    private double _FooterSpacing;

    /// <summary>
    /// Gets or sets the spacing of the bottom footer
    /// </summary>
    public double FooterSpacing
    {
        get => _FooterSpacing;
        set
        {
            _FooterSpacing = value;

            CodeEditBox.Padding = new Thickness(4, HeaderSpacing + 8, 8, value + 8);
            CodeEditBox.VerticalScrollBarMargin = new Thickness(0, HeaderSpacing, 0, value);
        }
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
            new(Brainf_ckThemes.VisualStudio));
}
