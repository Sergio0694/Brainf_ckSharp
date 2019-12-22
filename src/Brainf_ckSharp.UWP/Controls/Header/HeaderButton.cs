using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.UWP.Controls.Header
{
    /// <summary>
    /// A templated <see cref="Control"/> that acts as a header button in the shell
    /// </summary>
    [TemplatePart(Name = RootButtonName, Type = typeof(Button))]
    public sealed class HeaderButton : Control
    {
        private const string RootButtonName = "RootButton";

        /// <summary>
        /// The root <see cref="Button"/> control
        /// </summary>
        private Button? _RootButton;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _RootButton = (Button)GetTemplateChild(RootButtonName) ?? throw new InvalidOperationException($"Can't find {RootButtonName}");
        }

        /// <summary>
        /// Gets or sets the icon for the current control
        /// </summary>
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Icon"/>.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(string),
            typeof(HeaderButton),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the text for the current control
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Text"/>.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(HeaderButton),
            new PropertyMetadata(string.Empty));
    }
}
