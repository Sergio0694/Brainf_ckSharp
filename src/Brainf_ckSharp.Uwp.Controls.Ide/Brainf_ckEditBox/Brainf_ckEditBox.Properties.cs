using Windows.Foundation;
using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.EventArgs;
using Brainf_ckSharp.Uwp.Themes;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// Raised whenever the <see cref="PlainText"/> property changes
        /// </summary>
        public event TypedEventHandler<Brainf_ckEditBox, PlainTextChangedEventArgs>? PlainTextChanged;

        /// <summary>
        /// Gets the plain text currently displayed in the control
        /// </summary>
        public string PlainText
        {
            get => (string)GetValue(PlainTextProperty);
            private set
            {
                SetValue(PlainTextProperty, value);

                if (PlainTextChanged != null)
                {
                    PlainTextChangedEventArgs args = new PlainTextChangedEventArgs(value, _SyntaxValidationResult);
                    PlainTextChanged(this, args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the margin of the vertical scrolling bar for the control
        /// </summary>
        public Thickness VerticalScrollBarMargin
        {
            get => (Thickness)GetValue(VerticalScrollBarMarginProperty);
            set => SetValue(VerticalScrollBarMarginProperty, value);
        }

        /// <summary>
        /// Gets or sets whether or not to automatically indent brackets and parentheses
        /// </summary>
        public bool IsAutomaticBracketsIndentationEnabled
        {
            get => (bool)GetValue(IsAutomaticBracketsIndentationEnabledProperty);
            set => SetValue(IsAutomaticBracketsIndentationEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets the formatting style for brackets
        /// </summary>
        public BracketsFormattingStyle BracketsFormattingStyle
        {
            get => (BracketsFormattingStyle)GetValue(BracketsFormattingStyleProperty);
            set => SetValue(BracketsFormattingStyleProperty, value);
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
        /// Gets the dependency property for <see cref="PlainText"/>.
        /// </summary>
        public static readonly DependencyProperty PlainTextProperty =
            DependencyProperty.Register(
                nameof(PlainText),
                typeof(string),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata("\r"));

        /// <summary>
        /// Gets the dependency property for <see cref="VerticalScrollBarMargin"/>.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarMarginProperty =
            DependencyProperty.Register(
                nameof(VerticalScrollBarMargin),
                typeof(Thickness),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata(default(Thickness), OnVerticalScrollBarMarginPropertyChanged));

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
        /// Gets the dependency property for <see cref="BracketsFormattingStyle"/>.
        /// </summary>
        public static readonly DependencyProperty BracketsFormattingStyleProperty =
            DependencyProperty.Register(
                nameof(BracketsFormattingStyle),
                typeof(BracketsFormattingStyle),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata(default(BracketsFormattingStyle)));

        /// <summary>
        /// Gets the dependency property for <see cref="SyntaxHighlightTheme"/>.
        /// </summary>
        public static readonly DependencyProperty SyntaxHighlightThemeProperty =
            DependencyProperty.Register(
                nameof(SyntaxHighlightTheme),
                typeof(Brainf_ckTheme),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata(Brainf_ckThemes.VisualStudio));

        /// <summary>
        /// Updates the <see cref="FrameworkElement.Margin"/> property for <see cref="_VerticalContentScrollBar"/> when <see cref="VerticalScrollBarMargin"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="Brainf_ckEditBox"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance with the new <see cref="VerticalScrollBarMargin"/> value</param>
        private static void OnVerticalScrollBarMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Brainf_ckEditBox @this = (Brainf_ckEditBox)d;

            if (@this._VerticalContentScrollBar == null) return;

            @this._VerticalContentScrollBar.Margin = (Thickness)e.NewValue;
        }
    }
}
