using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

#nullable enable

namespace Brainf_ckSharp.Uwp.Behaviors;

/// <summary>
/// A behavior for a <see cref="Pivot"/> control to disable its touch gestures to change the selected page.
/// </summary>
public sealed class LockedPivotBehavior : Behavior<Pivot>
{
    /// <summary>
    /// The root <see cref="ScrollViewer"/> for the associated <see cref="Pivot"/>.
    /// </summary>
    private ScrollViewer? _scrollViewer;

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.Loaded += AssociatedObject_Loaded;
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.Loaded -= AssociatedObject_Loaded;

        if (this._scrollViewer is not null)
        {
            this._scrollViewer!.PointerEntered -= Scroller_PointerIn;
            this._scrollViewer.PointerMoved -= Scroller_PointerIn;
            this._scrollViewer.PointerExited -= Scroller_PointerOut;
            this._scrollViewer.PointerReleased -= Scroller_PointerOut;
            this._scrollViewer.PointerCaptureLost -= Scroller_PointerOut;
        }
    }

    // Initializes the logic when the control is loaded
    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
        if (((Pivot)sender).FindDescendant<ScrollViewer>(s => s.Name == "ScrollViewer") is not ScrollViewer scroller)
        {
            ThrowHelper.ThrowInvalidOperationException("Can't find parent scroller");

            return;
        }

        this._scrollViewer = scroller;

        // Add the handlers to disable the swipe gestures
        this._scrollViewer.PointerEntered += Scroller_PointerIn;
        this._scrollViewer.PointerMoved += Scroller_PointerIn;
        this._scrollViewer.PointerExited += Scroller_PointerOut;
        this._scrollViewer.PointerReleased += Scroller_PointerOut;
        this._scrollViewer.PointerCaptureLost += Scroller_PointerOut;
    }

    // Disables the swipe gesture for the keyboard pivot (swiping that pivot causes the app to crash)
    private void Scroller_PointerIn(object sender, PointerRoutedEventArgs e)
    {
        ((ScrollViewer)sender).HorizontalScrollMode = ScrollMode.Disabled;
    }

    // Restores the original scrolling settings when the pointer is outside the keyboard pivot
    private void Scroller_PointerOut(object sender, PointerRoutedEventArgs e)
    {
        ((ScrollViewer)sender).HorizontalScrollMode = ScrollMode.Enabled;
    }
}
