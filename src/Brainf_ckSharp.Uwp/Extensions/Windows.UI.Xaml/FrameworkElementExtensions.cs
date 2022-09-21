using System.Diagnostics.Contracts;
using Windows.Foundation;

namespace Windows.UI.Xaml;

/// <summary>
/// An extension <see langword="class"/> for the <see cref="FrameworkElement"/> type
/// </summary>
public static class FrameworkElementExtensions
{
    /// <summary>
    /// Gets the relative bounds of a given <see cref="FrameworkElement"/> relative to a parent element
    /// </summary>
    /// <param name="element">The target <see cref="FrameworkElement"/> instance</param>
    /// <param name="parent">The parent <see cref="UIElement"/> instance</param>
    /// <returns>The bounds of <paramref name="element"/> relative to <paramref name="parent"/></returns>
    [Pure]
    public static Rect GetRelativeBounds(this FrameworkElement element, UIElement parent)
    {
        try
        {
            // Get the element bounds and transform them relative to the parent element
            Rect bounds = new(0.0, 0.0, element.ActualWidth, element.ActualHeight);
            return element.TransformToVisual(parent).TransformBounds(bounds);
        }
        catch
        {
            // Fallback to an empty rect
            return default;
        }
    }
}
