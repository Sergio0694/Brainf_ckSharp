using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.UWP.Controls.InputPanel.VirtualKeyboard
{
    /// <summary>
    /// A templated <see cref="Control"/> for a Brainf*ck/PBrain operator
    /// </summary>
    [TemplatePart(Name = RootButtonName, Type = typeof(Button))]
    public sealed class OperatorButton : Control
    {
        // Constants for the template
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
            _RootButton.Click += RootButton_Click;
        }

        /// <summary>
        /// Gets or sets the operator for the current control
        /// </summary>
        public string Operator
        {
            get => (string)GetValue(OperatorProperty);
            set => SetValue(OperatorProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Operator"/>
        /// </summary>
        public static readonly DependencyProperty OperatorProperty = DependencyProperty.Register(
            nameof(Operator),
            typeof(string),
            typeof(OperatorButton),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the description for the current control
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Description"/>
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(OperatorButton),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Relays the <see cref="Button.Click"/> event for the root <see cref="Button"/>
        /// </summary>
        public event EventHandler? Click;

        // Updates the UI when the control is selected
        private void RootButton_Click(object sender, RoutedEventArgs e) => Click?.Invoke(this, EventArgs.Empty);
    }
}
