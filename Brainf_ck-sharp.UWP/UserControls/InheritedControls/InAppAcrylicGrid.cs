using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects.Base;

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
            SizeChanged += InAppAcrylicGrid_SizeChanged;
        }

        // Updates the size of the effect visual
        private void InAppAcrylicGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _Effect?.AdjustSize(e.NewSize.Width, e.NewSize.Height);
        }

        // Initializes the effect when the control is rendered for the first time
        private async void InAppAcrylicGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Children.Contains(_Win2DCanvas)) return;
            Children.Add(_Win2DCanvas);
            _Effect = await this.GetAttachedInAppSemiAcrylicEffectAsync(this, 8, 800,
                Color.FromArgb(byte.MaxValue, 34, 34, 34), 0.6f,
                _Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
        }

        // The canvas used to render part of the effect
        private readonly CanvasControl _Win2DCanvas;

        // Acrylic brush for the header
        private AttachedStaticCompositionEffect<InAppAcrylicGrid> _Effect;
    }
}
