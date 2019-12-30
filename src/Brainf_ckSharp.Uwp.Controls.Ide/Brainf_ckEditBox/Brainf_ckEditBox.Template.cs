using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ckSharp.Helpers;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using UICompositionAnimations.Enums;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    [TemplatePart(Name = BackgroundCanvasName, Type = typeof(Canvas))]
    [TemplatePart(Name = TextOverlaysCanvasName, Type = typeof(CanvasControl))]
    [TemplatePart(Name = SelectionHighlightBorderName, Type = typeof(Border))]
    [TemplatePart(Name = ContentScrollerName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = ContentElementName, Type = typeof(ScrollViewer))]
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// The name of the <see cref="Canvas"/> instance that holds the background controls
        /// </summary>
        private const string BackgroundCanvasName = "BackgroundCanvas";

        /// <summary>
        /// The name of the <see cref="CanvasControl"/> instance for the control
        /// </summary>
        private const string TextOverlaysCanvasName = "TextOverlaysCanvas";

        /// <summary>
        /// The name of the <see cref="Border"/> that highlights the current selection
        /// </summary>
        private const string SelectionHighlightBorderName = "SelectionHighlightBorder";

        /// <summary>
        /// The name of the <see cref="ScrollViewer"/> instance for the main content
        /// </summary>
        private const string ContentScrollerName = "ContentScroller";

        /// <summary>
        /// The name of the <see cref="ScrollViewer"/> instance for the main content
        /// </summary>
        private const string ContentElementName = "ContentElement";

        /// <summary>
        /// The name of the vertical <see cref="ScrollBar"/> instance for <see cref="_ContentElement"/>
        /// </summary>
        private const string VerticalScrollBarName = "VerticalScrollBar";

        /// <summary>
        /// The <see cref="Canvas"/> instance for the control
        /// </summary>
        private Canvas? _BackgroundCanvas;

        /// <summary>
        /// The <see cref="CanvasControl"/> instance for the control
        /// </summary>
        private CanvasControl? _TextOverlaysCanvas;

        /// <summary>
        /// The <see cref="Border"/> instance to highlight the selected line
        /// </summary>
        private Border? _SelectionHighlightBorder;

        /// <summary>
        /// The <see cref="ScrollViewer"/> instance for the main content
        /// </summary>
        private ScrollViewer? _ContentScroller;

        /// <summary>
        /// The <see cref="ContentPresenter"/> instance for the main content
        /// </summary>
        private ContentPresenter? _ContentElement;

        /// <summary>
        /// The vertical <see cref="ScrollBar"/> instance for the main content
        /// </summary>
        private ScrollBar? _VerticalContentScrollBar;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _BackgroundCanvas = (Canvas)GetTemplateChild(BackgroundCanvasName);
            _TextOverlaysCanvas = (CanvasControl)GetTemplateChild(TextOverlaysCanvasName);
            _SelectionHighlightBorder = (Border)GetTemplateChild(SelectionHighlightBorderName);
            _ContentScroller = (ScrollViewer)GetTemplateChild(ContentScrollerName);
            _ContentElement = (ContentPresenter)GetTemplateChild(ContentElementName);

            Guard.MustBeNotNull(_BackgroundCanvas, nameof(BackgroundCanvasName));
            Guard.MustBeNotNull(_TextOverlaysCanvas, nameof(TextOverlaysCanvasName));
            Guard.MustBeNotNull(_SelectionHighlightBorder, nameof(SelectionHighlightBorderName));
            Guard.MustBeNotNull(_ContentScroller, ContentScrollerName);
            Guard.MustBeNotNull(_ContentElement, ContentElementName);

            _BackgroundCanvas.SizeChanged += BackgroundCanvas_SizeChanged;
            _TextOverlaysCanvas.Draw += TextOverlaysCanvas_Draw;
            _ContentScroller.Loaded += ContentElement_Loaded;
            _ContentElement.SizeChanged += _ContentElement_SizeChanged;

            _ContentScroller.StartExpressionAnimation(_TextOverlaysCanvas, Axis.Y);
            _ContentScroller.StartExpressionAnimation(_TextOverlaysCanvas, Axis.X);
            _ContentScroller.StartExpressionAnimation(_SelectionHighlightBorder, Axis.Y);
        }

        /// <summary>
        /// A handler that is invoked whenever the background canvas changes size
        /// </summary>
        /// <param name="sender">The <see cref="Canvas"/> that hosts the text overlays</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> for <see cref="FrameworkElement.SizeChanged"/></param>
        private void BackgroundCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _SelectionHighlightBorder!.Width = e.NewSize.Width;
        }

        /// <summary>
        /// A handler that is invoked when <see cref="_ContentScroller"/> is loaded
        /// </summary>
        /// <param name="sender">The <see cref="ScrollViewer"/> for the main content</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> for <see cref="FrameworkElement.Loaded"/></param>
        private void ContentElement_Loaded(object sender, RoutedEventArgs e)
        {
            _VerticalContentScrollBar = (ScrollBar)_ContentScroller.FindDescendantByName(VerticalScrollBarName);

            Guard.MustBeNotNull(_VerticalContentScrollBar, nameof(_ContentScroller));

            _VerticalContentScrollBar.Margin = VerticalScrollBarMargin;
        }

        /// <summary>
        /// A handler that is invoked whenever the text renderer is resized
        /// </summary>
        /// <param name="sender">The <see cref="ContentPresenter"/> that hosts the text renderer</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> for <see cref="FrameworkElement.SizeChanged"/></param>
        private void _ContentElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /* This handler makes sure the Win2D canvas has enough space to render all of
             * its content. Since it's placed inside a canvas in the template, all its
             * area outside of the current viewport is still rendered, so that when
             * the expression animation scrolls the Win2D canvas around, all the text
             * overlays that were previously out of bounds can become visible.
             * The height is also considering the top padding set by the user,
             * plus 20 more DIPs just to be extra safe. */
            _TextOverlaysCanvas!.Height = e.NewSize.Height + Padding.Top + 20;
            _TextOverlaysCanvas.Width = e.NewSize.Width;
        }
    }
}
