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
    [TemplatePart(Name = HeaderContentPresenterName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = BorderElementName, Type = typeof(Border))]
    [TemplatePart(Name = TextOverlaysCanvasName, Type = typeof(CanvasControl))]
    [TemplatePart(Name = ContentScrollerName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = ContentElementName, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PlaceholderTextContentPresenterName, Type = typeof(TextBlock))]
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// The name of the <see cref="ContentPresenter"/> instance for the header
        /// </summary>
        private const string HeaderContentPresenterName = "HeaderContentPresenter";

        /// <summary>
        /// The name of the <see cref="Border"/> instance for the control
        /// </summary>
        private const string BorderElementName = "BorderElement";

        /// <summary>
        /// The name of the <see cref="CanvasControl"/> instance for the control
        /// </summary>
        private const string TextOverlaysCanvasName = "TextOverlaysCanvas";

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
        /// The name of the <see cref="TextBlock"/> instance for the placeholder
        /// </summary>
        private const string PlaceholderTextContentPresenterName = "PlaceholderTextContentPresenter";

        /// <summary>
        /// The <see cref="ContentPresenter"/> instance for the header
        /// </summary>
        private ContentPresenter? _HeaderContentPresenter;

        /// <summary>
        /// The <see cref="Border"/> instance for the control
        /// </summary>
        private Border? _BorderElement;

        /// <summary>
        /// The <see cref="CanvasControl"/> instance for the control
        /// </summary>
        private CanvasControl? _TextOverlaysCanvas;

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

        /// <summary>
        /// The <see cref="TextBlock"/> instance for the placeholder
        /// </summary>
        private TextBlock? _PlaceholderTextContentPresenter;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _HeaderContentPresenter = (ContentPresenter)GetTemplateChild(HeaderContentPresenterName);
            _BorderElement = (Border)GetTemplateChild(BorderElementName);
            _TextOverlaysCanvas = (CanvasControl)GetTemplateChild(TextOverlaysCanvasName);
            _ContentScroller = (ScrollViewer)GetTemplateChild(ContentScrollerName);
            _ContentElement = (ContentPresenter)GetTemplateChild(ContentElementName);
            _PlaceholderTextContentPresenter = (TextBlock)GetTemplateChild(PlaceholderTextContentPresenterName);

            Guard.MustBeNotNull(_HeaderContentPresenter, HeaderContentPresenterName);
            Guard.MustBeNotNull(_BorderElement, BorderElementName);
            Guard.MustBeNotNull(_TextOverlaysCanvas, nameof(TextOverlaysCanvasName));
            Guard.MustBeNotNull(_ContentScroller, ContentScrollerName);
            Guard.MustBeNotNull(_ContentElement, ContentElementName);
            Guard.MustBeNotNull(_PlaceholderTextContentPresenter, PlaceholderTextContentPresenterName);

            _ContentElement.SizeChanged += _ContentElement_SizeChanged;
            _TextOverlaysCanvas.Draw += TextOverlaysCanvas_Draw;
            _ContentScroller.Loaded += ContentElement_Loaded;

            _ContentScroller.StartExpressionAnimation(_TextOverlaysCanvas, Axis.Y);
            _ContentScroller.StartExpressionAnimation(_TextOverlaysCanvas, Axis.X);
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
