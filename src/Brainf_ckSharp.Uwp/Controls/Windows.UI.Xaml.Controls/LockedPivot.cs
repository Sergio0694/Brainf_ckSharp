using Microsoft.Toolkit.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Windows.UI.Xaml.Controls;

/// <summary>
/// A custom <see cref="Pivot"/> control that disables its touch gestures to change the selected page
/// </summary>
public sealed class LockedPivot : Pivot
{
    /// <summary>
    /// The <see cref="ScrollViewer"/> instance used by the current control
    /// </summary>
    private ScrollViewer? _Scroller;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (!(GetTemplateChild("ScrollViewer") is ScrollViewer scroller))
        {
            ThrowHelper.ThrowInvalidOperationException("Can't find parent scroller");

            return;
        }

        _Scroller = scroller;

        // Add the handlers to disable the swipe gestures
        scroller.PointerEntered += Scroller_PointerIn;
        scroller.PointerMoved += Scroller_PointerIn;
        scroller.PointerExited += Scroller_PointerOut;
        scroller.PointerReleased += Scroller_PointerOut;
        scroller.PointerCaptureLost += Scroller_PointerOut;
    }

    // Disables the swipe gesture for the keyboard pivot (swiping that pivot causes the app to crash)
    private void Scroller_PointerIn(object sender, PointerRoutedEventArgs e)
    {
        _Scroller!.HorizontalScrollMode = ScrollMode.Disabled;
    }

    // Restores the original scrolling settings when the pointer is outside the keyboard pivot
    private void Scroller_PointerOut(object sender, PointerRoutedEventArgs e)
    {
        _Scroller!.HorizontalScrollMode = ScrollMode.Enabled;
    }
}
