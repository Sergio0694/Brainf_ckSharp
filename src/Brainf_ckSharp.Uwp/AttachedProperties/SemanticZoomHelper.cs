using System.Collections;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

#nullable enable

namespace Brainf_ckSharp.Uwp.AttachedProperties;

/// <summary>
/// A <see langword="class"/> with an attached XAML property to facilitate the usage of the <see cref="SemanticZoom"/> control
/// </summary>
public static class SemanticZoomHelper
{
    /// <summary>
    /// Gets the value of <see cref="SourceProperty"/> for a given <see cref="SemanticZoom"/>
    /// </summary>
    /// <param name="element">The input <see cref="SemanticZoom"/> for which to get the property value</param>
    /// <returns>The value of the <see cref="SourceProperty"/> property for the input <see cref="SemanticZoom"/> instance</returns>
    public static IEnumerable GetSource(SemanticZoom element)
    {
        return (IEnumerable<IReadOnlyObservableGroup>)element.GetValue(SourceProperty);
    }

    /// <summary>
    /// Sets the value of <see cref="SourceProperty"/> for a given <see cref="SemanticZoom"/>
    /// </summary>
    /// <param name="element">The input <see cref="SemanticZoom"/> for which to set the property value</param>
    /// <param name="value">The value to set for the <see cref="double"/> property</param>
    public static void SetSource(SemanticZoom element, IEnumerable value)
    {
        element.SetValue(SourceProperty, value);
    }

    /// <summary>
    /// An attached property to easily set a grouped source collection for a <see cref="SemanticZoom"/> control
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
        "Source",
        typeof(IEnumerable),
        typeof(ItemsWrapGridHelper),
        new(DependencyProperty.UnsetValue, OnSourcePropertyChanged));

    /// <summary>
    /// Updates the UI when <see cref="SourceProperty"/> changes
    /// </summary>
    /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
    private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SemanticZoom @this = (SemanticZoom)d;

        if (!(@this.ZoomedInView is ListViewBase zoomedInList &&
              @this.ZoomedOutView is ListViewBase zoomedOutList))
        {
            return;
        }

        if (e.NewValue is IEnumerable value)
        {
            CollectionViewSource source = new CollectionViewSource
            {
                IsSourceGrouped = true,
                Source = value
            };

            // Assign the source data
            zoomedInList.ItemsSource = source.View;
            zoomedOutList.ItemsSource = source.View.CollectionGroups;
        }
        else zoomedInList.ItemsSource = zoomedOutList.ItemsSource = null;
    }
}
