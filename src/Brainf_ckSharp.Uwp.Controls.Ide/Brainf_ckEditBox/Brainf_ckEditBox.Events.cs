using System;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// Raised whenever the <see cref="Text"/> property changes
        /// </summary>
        public new event TypedEventHandler<Brainf_ckEditBox, TextChangedEventArgs>? TextChanged;

        /// <summary>
        /// Rasised when the cursor position changes
        /// </summary>
        public event TypedEventHandler<Brainf_ckEditBox, CursorPositionChangedEventArgs>? CursorPositionChanged;

        /// <summary>
        /// Raised before the overlays canvas is invalidated
        /// </summary>
        public event EventHandler? FormattingCompleted;

        /// <summary>
        /// Raised whenever the resources for the overlays are being created.
        /// </summary>
        public event TypedEventHandler<CanvasControl, CanvasCreateResourcesEventArgs>? CreateOverlayResources;

        /// <summary>T
        /// Raised whenever the overlays are being drawn. This event can be used to inject
        /// external overlays, that will automatically be kept in sync with the text displayed.
        /// </summary>
        public event TypedEventHandler<CanvasControl, CanvasDrawEventArgs>? DrawOverlays;

        /// <summary>
        /// Forces a redraw of the text overlays
        /// </summary>
        public void InvalidateOverlays()
        {
            _TextOverlaysCanvas!.Invalidate();
        }
    }
}
