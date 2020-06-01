using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Themes;
using Microsoft.Graphics.Canvas.Geometry;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// Gets the plain text currently displayed in the control
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            private set
            {
                SetValue(TextProperty, value);

                TextChanged?.Invoke(this, new TextChangedEventArgs(value, _SyntaxValidationResult));
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
        /// Gets or sets the syntax highlight theme to use
        /// </summary>
        public FrameworkElement ContextMenuSecondaryContent
        {
            get => (FrameworkElement)GetValue(ContextMenuSecondaryContentProperty);
            set => SetValue(ContextMenuSecondaryContentProperty, value);
        }

        /// <summary>
        /// Gets the dependency property for <see cref="Text"/>.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
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
                new PropertyMetadata(Brainf_ckThemes.VisualStudio, OnSyntaxHighlightThemePropertyChanged));

        /// <summary>
        /// Gets the dependency property for <see cref="ContextMenuSecondaryContent"/>.
        /// </summary>
        public static readonly DependencyProperty ContextMenuSecondaryContentProperty =
            DependencyProperty.Register(
                nameof(ContextMenuSecondaryContent),
                typeof(FrameworkElement),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata(null, OnContextMenuSecondaryContentPropertyChanged));

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

        /// <summary>
        /// Updates the Win2D properties when <see cref="SyntaxHighlightThemeProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="Brainf_ckEditBox"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance with the new <see cref="SyntaxHighlightTheme"/> value</param>
        private static void OnSyntaxHighlightThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Brainf_ckEditBox @this = (Brainf_ckEditBox)d;
            Brainf_ckTheme theme = (Brainf_ckTheme)e.NewValue;

            // Column guides color and dash style
            @this._DashStrokeColor = theme.BracketsGuideColor;

            if (theme.BracketsGuideStrokesLength is int dashLength)
            {
                @this._DashStrokeStyle = new CanvasStrokeStyle { CustomDashStyle = new float[] { 2, 2 + dashLength } };
            }
            else @this._DashStrokeStyle = new CanvasStrokeStyle();

            // Try to update the theme
            if (@this.TryUpdateVisualElementsOnThemeChanged(theme))
            {
                // Refresh the syntax highlight
                @this.ApplySyntaxHighlight();
            }
        }

        /// <summary>
        /// Updates the flyout UI when when <see cref="ContextMenuSecondaryContent"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="Brainf_ckEditBox"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance with the new <see cref="ContextMenuSecondaryContent"/> value</param>
        private static void OnContextMenuSecondaryContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Brainf_ckEditBox @this = (Brainf_ckEditBox)d;
            FrameworkElement? content = (FrameworkElement)e.NewValue;
            CommandBarFlyout flyout = (CommandBarFlyout)@this.ContextFlyout;
            Visibility visibility = content is null ? Visibility.Collapsed : Visibility.Visible;

            ((AppBarButton)flyout.PrimaryCommands[0]).Visibility = visibility;
            ((AppBarElementContainer)flyout.SecondaryCommands[0]).Visibility = visibility;
            ((AppBarElementContainer)flyout.SecondaryCommands[0]).Content = content;
        }
    }
}
