using Windows.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Diagnostics;

namespace Brainf_ckSharp.Uwp.AttachedProperties;

/// <summary>
/// An attached property that sets the <see cref="ObservableRecipient.IsActive"/> property for the data context of a given <see cref="FrameworkElement"/>
/// </summary>
public static class ObservableRecipientHelper
{
    /// <summary>
    /// Gets the value of <see cref="IsActiveProperty"/> for a given <see cref="FrameworkElement"/>
    /// </summary>
    /// <param name="element">The input <see cref="FrameworkElement"/> for which to get the property value</param>
    /// <returns>The value of the <see cref="IsActiveProperty"/> property for the input <see cref="FrameworkElement"/> instance</returns>
    public static bool GetIsActive(FrameworkElement element)
    {
        return (bool)element.GetValue(IsActiveProperty);
    }

    /// <summary>
    /// Sets the value of <see cref="IsActiveProperty"/> for a given <see cref="FrameworkElement"/>
    /// </summary>
    /// <param name="element">The input <see cref="FrameworkElement"/> for which to set the property value</param>
    /// <param name="value">The value to set for the <see cref="IsActiveProperty"/> property</param>
    public static void SetIsActive(FrameworkElement element, bool value)
    {
        element?.SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// An attached property that controls the <see cref="ObservableRecipient.IsActive"/> property for the data context of a given <see cref="FrameworkElement"/>
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
        nameof(ObservableRecipient.IsActive),
        typeof(bool),
        typeof(ObservableRecipientHelper),
        new(DependencyProperty.UnsetValue, OnIsActivePropertyChanged));

    /// <summary>
    /// Updates the UI when <see cref="IsActiveProperty"/> changes
    /// </summary>
    /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
    private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement @this = (FrameworkElement)d;
        bool value = (bool)e.NewValue;

        if (@this.DataContext is ObservableRecipient viewModel)
        {
            viewModel.IsActive = value;
        }
        else ThrowHelper.ThrowInvalidOperationException("Invalid view model type");
    }
}