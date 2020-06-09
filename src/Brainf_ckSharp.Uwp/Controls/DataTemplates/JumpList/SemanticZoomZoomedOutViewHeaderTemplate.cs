using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Brainf_ckSharp.Uwp.Controls.DataTemplates.JumpList
{
    /// <summary>
    /// A <see cref="Control"/> that represents a zoomed out template for a grouped collection
    /// </summary>
    [TemplatePart(Name = "DescriptionBlock", Type = typeof(TextBlock))]
    public sealed class SemanticZoomZoomedOutViewHeaderTemplate : Control
    {
        /// <summary>
        /// The <see cref="TextBlock"/> displaying the description
        /// </summary>
        private TextBlock _DescriptionBlock;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _DescriptionBlock = GetTemplateChild("DescriptionBlock") as TextBlock
                                ?? throw new InvalidOperationException("Failed to find description block");

            // Load the span explicitly, if present
            if (DescriptionSpan is { } span)
            {
                _DescriptionBlock.Inlines.Clear();
                _DescriptionBlock.Inlines.Add(span);
            }
        }

        /// <summary>
        /// Gets or sets the title for the current control
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Title"/>
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(SemanticZoomZoomedOutViewHeaderTemplate),
            new PropertyMetadata(DependencyProperty.UnsetValue));

        /// <summary>
        /// Gets or sets the text for the current control's description
        /// </summary>
        public string DescriptionText
        {
            get => (string)GetValue(DescriptionTextProperty);
            set => SetValue(DescriptionTextProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="DescriptionText"/>
        /// </summary>
        public static readonly DependencyProperty DescriptionTextProperty = DependencyProperty.Register(
            nameof(DescriptionText),
            typeof(string),
            typeof(SemanticZoomZoomedOutViewHeaderTemplate),
            new PropertyMetadata(null, OnDescriptionTextPropertyChanged));

        /// <summary>
        /// Sets a new <see cref="Span"/> for the description when <see cref="DescriptionText"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnDescriptionTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SemanticZoomZoomedOutViewHeaderTemplate @this = (SemanticZoomZoomedOutViewHeaderTemplate)d;

            @this.DescriptionSpan = new Span { Inlines = { new Run { Text = (string)e.NewValue ?? string.Empty } } };
        }

        /// <summary>
        /// Gets or sets the <see cref="Span"/> for the current control's description
        /// </summary>
        public Span DescriptionSpan
        {
            get => (Span)GetValue(DescriptionSpanProperty);
            set => SetValue(DescriptionSpanProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="DescriptionSpan"/>
        /// </summary>
        public static readonly DependencyProperty DescriptionSpanProperty = DependencyProperty.Register(
            nameof(DescriptionSpan),
            typeof(Span),
            typeof(SemanticZoomZoomedOutViewHeaderTemplate),
            new PropertyMetadata(null, OnDescriptionSpanPropertyChanged));

        /// <summary>
        /// Sets a new <see cref="Span"/> for the description when <see cref="DescriptionSpan"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnDescriptionSpanPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SemanticZoomZoomedOutViewHeaderTemplate @this = (SemanticZoomZoomedOutViewHeaderTemplate)d;
            Span span = e.NewValue as Span
                        ?? throw new ArgumentException($"Can't assign null to the {nameof(DescriptionSpan)} property");

            if (@this._DescriptionBlock is null) return;

            @this._DescriptionBlock.Inlines.Clear();
            @this._DescriptionBlock.Inlines.Add(span);
        }
    }
}
