using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Host.Header
{
    /// <summary>
    /// A templated <see cref="Control"/> that acts as a header button in the shell
    /// </summary>
    [TemplatePart(Name = RootButtonName, Type = typeof(Button))]
    public sealed class HeaderButton : Control
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

            UpdateVisualState();
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
            typeof(HeaderButton),
            new PropertyMetadata(DependencyProperty.UnsetValue));

        /// <summary>
        /// Gets or sets the text for the current control
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Text"/>
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(HeaderButton),
            new PropertyMetadata(DependencyProperty.UnsetValue));

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
            typeof(HeaderButton),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnIsSelectedPropertyChanged));

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
            HeaderButton @this = (HeaderButton)d;

            if (e.NewValue is bool value && value) @this.Selected?.Invoke(@this, EventArgs.Empty);
            @this.UpdateVisualState();
        }

        /// <summary>
        /// Applies the correct visual state when the <see cref="IsSelected"/> property changes
        /// </summary>
        /// <remarks>This method also needs to be called when the template is applied</remarks>
        private void UpdateVisualState()
        {
            string name = IsSelected ? SelectedVisualStateName : DefaultVisualStateName;
            VisualStateManager.GoToState(this, name, false);
        }

        // Updates the UI when the control is selected
        private void RootButton_Click(object sender, RoutedEventArgs e) => IsSelected = true;
    }
}
