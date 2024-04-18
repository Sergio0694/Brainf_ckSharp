using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.AttachedProperties;

/// <summary>
/// A <see langword="class"/> with an attached XAML property to control the auto scrolling on a target <see cref="ItemsWrapGrid"/> control
/// </summary>
public static class ItemsWrapGridHelper
{
    /// <summary>
    /// Gets the value of <see cref="DesiredItemWidthProperty"/> for a given <see cref="ItemsWrapGrid"/>
    /// </summary>
    /// <param name="element">The input <see cref="ListViewBase"/> for which to get the property value</param>
    /// <returns>The value of the <see cref="DesiredItemWidthProperty"/> property for the input <see cref="ItemsWrapGrid"/> instance</returns>
    public static double GetDesiredItemWidth(ItemsWrapGrid element)
    {
        return (double)element.GetValue(DesiredItemWidthProperty);
    }

    /// <summary>
    /// Sets the value of <see cref="DesiredItemWidthProperty"/> for a given <see cref="ItemsWrapGrid"/>
    /// </summary>
    /// <param name="element">The input <see cref="UIElement"/> for which to set the property value</param>
    /// <param name="value">The value to set for the <see cref="double"/> property</param>
    public static void SetDesiredItemWidth(ItemsWrapGrid element, double value)
    {
        element.SetValue(DesiredItemWidthProperty, value);
    }

    /// <summary>
    /// An attached property that indicates whether a given element has an active blinking animation
    /// </summary>
    public static readonly DependencyProperty DesiredItemWidthProperty = DependencyProperty.RegisterAttached(
        "DesiredItemWidth",
        typeof(double),
        typeof(ItemsWrapGridHelper),
        new(DependencyProperty.UnsetValue, OnDesiredItemWidthPropertyChanged));

    /// <summary>
    /// A table that keeps track of <see cref="ItemsWrapGrid"/> instances with an already added <see cref="FrameworkElement.SizeChanged"/> handler for <see cref="DesiredItemWidthProperty"/>
    /// </summary>
    private static readonly ConditionalWeakTable<ItemsWrapGrid, object?> ControlsMap = [];

    /// <summary>
    /// Updates the UI when <see cref="DesiredItemWidthProperty"/> changes
    /// </summary>
    /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
    private static void OnDesiredItemWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ItemsWrapGrid @this = (ItemsWrapGrid)d;

        // Only add the handler the first time
        if (!ControlsMap.TryGetValue(@this, out _))
        {
            ControlsMap.Add(@this, null);
            @this.SizeChanged += ItemsWrapGrid_SizeChanged;
        }
    }

    /// <summary>
    /// Adjusts the <see cref="ItemsWrapGrid.ItemWidth"/> property when resizing the target <see cref="ItemsWrapGrid"/> instance
    /// </summary>
    /// <param name="sender">The source <see cref="ListViewBase"/> instance</param>
    /// <param name="e">The args for the current event</param>
    private static void ItemsWrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ItemsWrapGrid @this = (ItemsWrapGrid)sender;

        double
            desiredWidth = GetDesiredItemWidth(@this),
            columns = Math.Ceiling(e.NewSize.Width / desiredWidth);

        int maximumRowsOrColumns = @this.MaximumRowsOrColumns;
        if (maximumRowsOrColumns > 0)
        {
            columns = Math.Min(columns, maximumRowsOrColumns);
        }

        @this.ItemWidth = e.NewSize.Width / columns;
    }
}
