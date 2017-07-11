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
            Loaded += InAppAcrylicGrid_Loaded;
        }

        /// <summary>
        /// Gets or sets whether or not native resources should be disposed when the control is unloaded
        /// </summary>
        public bool DisposeOnUnload { get; set; }

        // Indicates whether or not the optional unload handler has been set up
        private bool _UnloadHandlerAdded;

        // Initializes the effect when the control is rendered for the first time
        private async void InAppAcrylicGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // UI setup
            _Win2DCanvas = new CanvasControl();
            if (Children.Contains(_Win2DCanvas)) return;
            Children.Add(_Win2DCanvas);
            await this.AttachCompositionInAppCustomAcrylicEffectAsync(this, 8, 800,
                Color.FromArgb(byte.MaxValue, 34, 34, 34), 0.6f, null,
                _Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"), disposeOnUnload: DisposeOnUnload);

            // Dispose setup
            if (DisposeOnUnload && !_UnloadHandlerAdded)
            {
                _UnloadHandlerAdded = true;
                Unloaded += (s, _) =>
                {
                    _Win2DCanvas.RemoveFromVisualTree();
                    _Win2DCanvas = null;
                };
            }
        }

        // The canvas used to render part of the effect
        private CanvasControl _Win2DCanvas;
    }
}
