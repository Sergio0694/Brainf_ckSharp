using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ckSharp.Shared.ViewModels.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Brainf_ckSharp.Uwp.Controls.Host.InputPanel;

public sealed partial class StdinHeader : UserControl
{
    public StdinHeader()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<StdinHeaderViewModel>();

        ViewModel.IsActive = true;
    }

    /// <summary>
    /// Gets the <see cref="StdinHeaderViewModel"/> instance currently in use
    /// </summary>
    public StdinHeaderViewModel ViewModel => (StdinHeaderViewModel)DataContext;

    /// <summary>
    /// Gets or sets the currently selected index for the shell
    /// </summary>
    public int ShellSelectedIndex
    {
        get => (int)GetValue(ShellSelectedIndexProperty);
        set => SetValue(ShellSelectedIndexProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="ShellSelectedIndex"/>.
    /// </summary>
    public static readonly DependencyProperty ShellSelectedIndexProperty = DependencyProperty.Register(
        nameof(ShellSelectedIndex),
        typeof(int),
        typeof(StdinHeader),
        new(default(int)));

    /// <summary>
    /// Gets or sets the currently selected index for the stdin header buttons
    /// </summary>
    public int StdinSelectedIndex
    {
        get => (int)GetValue(StdinSelectedIndexProperty);
        set => SetValue(StdinSelectedIndexProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="StdinSelectedIndex"/>.
    /// </summary>
    public static readonly DependencyProperty StdinSelectedIndexProperty = DependencyProperty.Register(
        nameof(StdinSelectedIndex),
        typeof(int),
        typeof(StdinHeader),
        new(default(int), OnStdinSelectedIndexPropertyChanged));

    /// <summary>
    /// Updates the UI when <see cref="StdinSelectedIndex"/> changes
    /// </summary>
    /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
    private static void OnStdinSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        StdinHeader @this = (StdinHeader)d;
        int index = (int)e.NewValue;
        VisualStateManager.GoToState(@this, index == 0 ? nameof(KeyboardSelectedState) : nameof(MemoryViewerSelectedState), false);
    }

    // Sets the selected index to 0 when the keyboard button is clicked
    private void VirtualKeyboardHeaderSelected(object sender, EventArgs e)
    {
        StdinSelectedIndex = 0;
    }

    // Sets the selected index to 1 when the memory viewer button is clicked
    private void MemoryViewerHeaderSelected(object sender, EventArgs e)
    {
        StdinSelectedIndex = 1;
    }

    // Sets the selected index to 0 when the memory viewer button is deselected
    private void MemoryViewerHeaderDeselected(object sender, EventArgs e)
    {
        StdinSelectedIndex = 0;
    }

    // Prevents the event from bubbling up the UI stack
    private void StdinBox_OnCharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
    {
        args.Handled = true;
    }
}
