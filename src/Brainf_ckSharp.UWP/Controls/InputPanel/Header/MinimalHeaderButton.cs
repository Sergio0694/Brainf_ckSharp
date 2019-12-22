using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.UWP.Controls.InputPanel.Header
{
    /// <summary>
    /// A templated <see cref="Control"/> that acts as a minimal header button in the stdin header
    /// </summary>
    [TemplatePart(Name = RootButtonName, Type = typeof(Button))]
    public sealed class MinimalHeaderButton : Control
    {
        // Constants for the template
        private const string RootButtonName = "RootButton";
        private const string DefaultVisualStateName = "Default";
        private const string SelectedVisualStateName = "Selected";

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
        /// Gets or sets the icon for the current control
        /// </summary>
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Icon"/>
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(string),
            typeof(MinimalHeaderButton),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets whether or not the control is currently selected
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="IsSelected"/>
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(MinimalHeaderButton),
            new PropertyMetadata(default(bool), OnIsSelectedPropertyChanged));

        /// <summary>
        /// Raised whenever the <see cref="IsSelected"/> property is set to <see langword="true"/>
        /// </summary>
        public event EventHandler? Selected;

        /// <summary>
        /// Updates the UI when <see cref="IsSelected"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MinimalHeaderButton @this = (MinimalHeaderButton)d;
            if (e.NewValue is bool value && value)
            {
                VisualStateManager.GoToState(@this, SelectedVisualStateName, false);
                @this.Selected?.Invoke(@this, EventArgs.Empty);
            }
            else VisualStateManager.GoToState(@this, DefaultVisualStateName, false);
        }

        // Updates the UI when the control is selected
        private void RootButton_Click(object sender, RoutedEventArgs e) => IsSelected = true;
    }
}
