using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.Uwp.Controls.Host.Header;

public sealed partial class HeaderPanel : UserControl
{
    public HeaderPanel()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the currently selected index for the header buttons
    /// </summary>
    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="SelectedIndex"/>.
    /// </summary>
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
        nameof(SelectedIndex),
        typeof(int),
        typeof(HeaderPanel),
        new(default(int), OnSelectedIndexPropertyChanged));

    /// <summary>
    /// Updates the UI when <see cref="SelectedIndex"/> changes
    /// </summary>
    /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
    private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        HeaderPanel @this = (HeaderPanel)d;
        int index = (int)e.NewValue;

        VisualStateManager.GoToState(@this, index == 0 ? nameof(ConsoleSelectedState) : nameof(IdeSelectedState), false);
    }

    // Sets the selected index to 0 when the console button is clicked
    private void ConsoleHeaderSelected(object sender, EventArgs e) => SelectedIndex = 0;

    // Sets the selected index to 1 when the IDE button is clicked
    private void IdeHeaderSelected(object sender, EventArgs e) => SelectedIndex = 1;

    /// <summary>
    /// Sets the loaded control as the draggable region for the title bar
    /// </summary>
    /// <param name="sender">The <see cref="UIElement"/> to assign as the draggable area of the title bar</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance for the current event</param>
    private void DragBorder_OnLoaded(object sender, RoutedEventArgs e)
    {
        Window.Current.SetTitleBar((UIElement)sender);
    }
}
