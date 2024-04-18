using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Brainf_ckSharp.Uwp.Themes;
using Brainf_ckSharp.Uwp.Themes.Enums;
using Microsoft.Graphics.Canvas.UI.Xaml;
using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide;

[TemplatePart(Name = BackgroundCanvasName, Type = typeof(Canvas))]
[TemplatePart(Name = TextOverlaysCanvasName, Type = typeof(CanvasControl))]
[TemplatePart(Name = SelectionHighlightBorderName, Type = typeof(Border))]
[TemplatePart(Name = CursorIndicatorRectangleName, Type = typeof(Rectangle))]
[TemplatePart(Name = SyntaxErrorToolTipName, Type = typeof(TeachingTip))]
[TemplatePart(Name = ContentScrollerName, Type = typeof(ContentPresenter))]
[TemplatePart(Name = ContentElementName, Type = typeof(ScrollViewer))]
internal sealed partial class Brainf_ckEditBox
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
    /// The name of the <see cref="Rectangle"/> that indicates the position of the cursor
    /// </summary>
    private const string CursorIndicatorRectangleName = "CursorIndicatorRectangle";

    /// <summary>
    /// The name of the <see cref="TeachingTip"/> that indicates syntax errors
    /// </summary>
    private const string SyntaxErrorToolTipName = "SyntaxErrorToolTip";

    /// <summary>
    /// The name of the <see cref="ScrollViewer"/> instance for the main content
    /// </summary>
    private const string ContentScrollerName = "ContentScroller";

    /// <summary>
    /// The name of the <see cref="ScrollViewer"/> instance for the main content
    /// </summary>
    private const string ContentElementName = "ContentElement";

    /// <summary>
    /// The name of the vertical <see cref="ScrollBar"/> instance for <see cref="ContentElement"/>
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
    /// The <see cref="Rectangle"/> instance to indicate the cursor position
    /// </summary>
    private Rectangle? _CursorIndicatorRectangle;

    private TeachingTip? _SyntaxErrorToolTip;

    /// <summary>
    /// The vertical <see cref="ScrollBar"/> instance for the main content
    /// </summary>
    private ScrollBar? _VerticalContentScrollBar;

    /// <summary>
    /// Gets the <see cref="ContentPresenter"/> instance for the main content
    /// </summary>
    public ContentPresenter? ContentElement { get; private set; }

    /// <summary>
    /// Gets the <see cref="ScrollViewer"/> instance for the main content
    /// </summary>
    public ScrollViewer? ContentScroller { get; private set; }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this._BackgroundCanvas = (Canvas)GetTemplateChild(BackgroundCanvasName);
        this._TextOverlaysCanvas = (CanvasControl)GetTemplateChild(TextOverlaysCanvasName);
        this._SelectionHighlightBorder = (Border)GetTemplateChild(SelectionHighlightBorderName);
        this._CursorIndicatorRectangle = (Rectangle)GetTemplateChild(CursorIndicatorRectangleName);
        this._SyntaxErrorToolTip = (TeachingTip)GetTemplateChild(SyntaxErrorToolTipName);
        ContentScroller = (ScrollViewer)GetTemplateChild(ContentScrollerName);
        ContentElement = (ContentPresenter)GetTemplateChild(ContentElementName);

        Guard.IsNotNull(this._BackgroundCanvas, nameof(BackgroundCanvasName));
        Guard.IsNotNull(this._TextOverlaysCanvas, nameof(TextOverlaysCanvasName));
        Guard.IsNotNull(this._SelectionHighlightBorder, nameof(SelectionHighlightBorderName));
        Guard.IsNotNull(this._CursorIndicatorRectangle, nameof(CursorIndicatorRectangleName));
        Guard.IsNotNull(this._SyntaxErrorToolTip, SyntaxErrorToolTipName);
        Guard.IsNotNull(ContentScroller, ContentScrollerName);
        Guard.IsNotNull(ContentElement, ContentElementName);

        this._BackgroundCanvas.SizeChanged += BackgroundCanvas_SizeChanged;
        this._TextOverlaysCanvas.CreateResources += _TextOverlaysCanvas_CreateResources;
        this._TextOverlaysCanvas.Draw += TextOverlaysCanvas_Draw;
        this._SyntaxErrorToolTip.Closed += delegate { ContentScroller.IsHitTestVisible = true; };
        ContentScroller.Loaded += ContentElement_Loaded;
        ContentElement.SizeChanged += ContentElement_SizeChanged;

        ContentScroller.StartExpressionAnimation(this._TextOverlaysCanvas, Axis.X, VisualProperty.Offset);
        ContentScroller.StartExpressionAnimation(this._TextOverlaysCanvas, Axis.Y, VisualProperty.Offset);
        ContentScroller.StartExpressionAnimation(this._SelectionHighlightBorder, Axis.Y, VisualProperty.Offset);
        ContentScroller.StartExpressionAnimation(this._CursorIndicatorRectangle, Axis.X, VisualProperty.Offset);
        ContentScroller.StartExpressionAnimation(this._CursorIndicatorRectangle, Axis.Y, VisualProperty.Offset);

        UpdateVisualElementsOnThemeChanged(SyntaxHighlightTheme);
    }

    /// <summary>
    /// A handler that is invoked whenever the background canvas changes size
    /// </summary>
    /// <param name="sender">The <see cref="Canvas"/> that hosts the text overlays</param>
    /// <param name="e">The <see cref="SizeChangedEventArgs"/> for <see cref="FrameworkElement.SizeChanged"/></param>
    private void BackgroundCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        this._SelectionHighlightBorder!.Width = e.NewSize.Width;
    }

    /// <summary>
    /// A handler that is invoked when <see cref="ContentScroller"/> is loaded
    /// </summary>
    /// <param name="sender">The <see cref="ScrollViewer"/> for the main content</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> for <see cref="FrameworkElement.Loaded"/></param>
    private void ContentElement_Loaded(object sender, RoutedEventArgs e)
    {
        this._VerticalContentScrollBar = (ScrollBar?)ContentScroller!.FindDescendant(VerticalScrollBarName);

        Guard.IsNotNull(this._VerticalContentScrollBar, nameof(ContentScroller));

        this._VerticalContentScrollBar.Margin = VerticalScrollBarMargin;
    }

    /// <summary>
    /// A handler that is invoked whenever the text renderer is resized
    /// </summary>
    /// <param name="sender">The <see cref="ContentPresenter"/> that hosts the text renderer</param>
    /// <param name="e">The <see cref="SizeChangedEventArgs"/> for <see cref="FrameworkElement.SizeChanged"/></param>
    private void ContentElement_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // This handler makes sure the Win2D canvas has enough space to render all of
        // its content. Since it's placed inside a canvas in the template, all its
        // area outside of the current viewport is still rendered, so that when
        // the expression animation scrolls the Win2D canvas around, all the text
        // overlays that were previously out of bounds can become visible.
        // The height is also considering the top padding set by the user,
        // plus 20 more DIPs just to be extra safe.
        this._TextOverlaysCanvas!.Height = e.NewSize.Height + Padding.Top + 20;
        this._TextOverlaysCanvas.Width = e.NewSize.Width;
    }

    /// <summary>
    /// Tries to update the visual elements in the template when the current <see cref="Brainf_ckTheme"/> changes
    /// </summary>
    /// <param name="theme">The <see cref="Brainf_ckTheme"/> to use</param>
    /// <returns>Whether or not the theme change was applied</returns>
    private bool TryUpdateVisualElementsOnThemeChanged(Brainf_ckTheme theme)
    {
        if (this._SelectionHighlightBorder is null)
        {
            return false;
        }

        UpdateVisualElementsOnThemeChanged(theme);

        return true;
    }

    /// <summary>
    /// Updates the visual elements in the template when the current <see cref="Brainf_ckTheme"/> changes
    /// </summary>
    /// <param name="theme">The <see cref="Brainf_ckTheme"/> to use</param>
    private void UpdateVisualElementsOnThemeChanged(Brainf_ckTheme theme)
    {
        switch (theme.LineHighlightStyle)
        {
            case LineHighlightStyle.Outline:
                this._SelectionHighlightBorder!.BorderThickness = new Thickness(2);
                this._SelectionHighlightBorder.BorderBrush = new SolidColorBrush(theme.LineHighlightColor);
                this._SelectionHighlightBorder.Background = null;
                break;
            case LineHighlightStyle.Fill:
                this._SelectionHighlightBorder!.BorderThickness = default;
                this._SelectionHighlightBorder.BorderBrush = null;
                this._SelectionHighlightBorder.Background = new SolidColorBrush(theme.LineHighlightColor);
                break;
            default:
                ThrowHelper.ThrowArgumentException("Invalid line highlight style");
                break;
        }
    }
}
