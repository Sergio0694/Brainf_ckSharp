using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A custom <see cref="Pivot"/> control that disables its touch gestures to change the selected page
    /// </summary>
    public class LockedPivot : Pivot
    {
        private ScrollViewer _Scroller;

        protected override void OnApplyTemplate()
        {
            // Disable the touch gestures
            base.OnApplyTemplate();
            if (GetTemplateChild("ScrollViewer") is ScrollViewer scroller)
            {
                _Scroller = scroller;
                scroller.PointerEntered += Scroller_PointerIn;
                scroller.PointerMoved += Scroller_PointerIn;
                scroller.PointerExited += Scroller_PointerOut;
                scroller.PointerReleased += Scroller_PointerOut;
                scroller.PointerCaptureLost += Scroller_PointerOut;
            }
            else throw new InvalidOperationException("Invalid Pivot template");
        }

        // Disables the swipe gesture for the keyboard pivot (swiping that pivot causes the app to crash)
        private void Scroller_PointerIn(object sender, PointerRoutedEventArgs e)
        {
            _Scroller.HorizontalScrollMode = ScrollMode.Disabled;
        }

        // Restores the original scrolling settings when the pointer is outside the keyboard pivot
        private void Scroller_PointerOut(object sender, PointerRoutedEventArgs e)
        {
            _Scroller.HorizontalScrollMode = ScrollMode.Enabled;
        }
    }
}
