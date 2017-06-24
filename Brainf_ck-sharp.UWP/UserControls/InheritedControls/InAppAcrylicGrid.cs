using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using UICompositionAnimations.Behaviours;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A <see cref="Grid"/> that automatically includes an in-app acrylic blur effect
    /// </summary>
    public class InAppAcrylicGrid : Grid
    {
        public InAppAcrylicGrid()
        {
            IsHitTestVisible = false;
            _Win2DCanvas = new CanvasControl();
            Loaded += InAppAcrylicGrid_Loaded;
        }

        // Initializes the effect when the control is rendered for the first time
        private async void InAppAcrylicGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Children.Contains(_Win2DCanvas)) return;
            Children.Add(_Win2DCanvas);
            await this.AttachCompositionInAppCustomAcrylicEffectAsync(this, 8, 800,
                Color.FromArgb(byte.MaxValue, 34, 34, 34), 0.6f, null,
                _Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
        }

        // The canvas used to render part of the effect
        private readonly CanvasControl _Win2DCanvas;
    }
}
