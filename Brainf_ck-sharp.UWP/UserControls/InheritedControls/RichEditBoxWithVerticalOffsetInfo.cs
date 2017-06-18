using System;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A RichEditBox that provides info on its inner ScrollViewer control
    /// </summary>
    public class RichEditBoxWithVerticalOffsetInfo : RichEditBox
    {
        public RichEditBoxWithVerticalOffsetInfo()
        {
            Loaded += (s, e) =>
            {
                _TemplateScrollBar = _TemplateScrollViewer.FindChild<ScrollBar>();
                _TemplateScrollBar.Margin = ScrollBarMargin;
            };
        }

        /// <summary>
        /// Gets the internal <see cref="ScrollBar"/> for the <see cref="ScrollViewer"/> that hosts the content
        /// </summary>
        private ScrollBar _TemplateScrollBar;

        /// <summary>
        /// Gets or sets the margin of the internal <see cref="ScrollBar"/> control
        /// </summary>
        public Thickness ScrollBarMargin
        {
            get => (Thickness)GetValue(ScrollBarMarginProperty);
            set => SetValue(ScrollBarMarginProperty, value);
        }

        public static readonly DependencyProperty ScrollBarMarginProperty = DependencyProperty.Register(
            nameof(ScrollBarMargin), typeof(Thickness), typeof(RichEditBoxWithVerticalOffsetInfo), 
            new PropertyMetadata(default(Thickness), OnScrollBarMarginPropertyChanged));

        private static void OnScrollBarMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollBar bar = d.To<RichEditBoxWithVerticalOffsetInfo>()._TemplateScrollBar;
            if (bar != null) bar.Margin = e.NewValue.To<Thickness>();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _TemplateScrollViewer = GetTemplateChild("ContentScroller") as ScrollViewer;
        }

        /// <summary>
        /// Gets the inner ScrollViewer, once the control has been added to the visual tree and loaded
        /// </summary>
        private ScrollViewer _TemplateScrollViewer;

        /// <summary>
        /// Gets the curent vertical offset of the inner ScrollViewer
        /// </summary>
        public double VerticalScrollViewerOffset => _TemplateScrollViewer.VerticalOffset;

        /// <summary>
        /// Gets the curent horizontal offset of the inner ScrollViewer
        /// </summary>
        public double HorizontalScrollViewerOffset => _TemplateScrollViewer.HorizontalOffset;

        /// <summary>
        /// Gets the actual vertical offset of the current text selection
        /// </summary>
        public Point ActualSelectionVerticalOffset
        {
            get
            {
                Document.Selection.GetRect(PointOptions.Transform, out Rect textRect, out _);
                return new Point(textRect.X, textRect.Top - VerticalScrollViewerOffset);
            }
        }

        /// <summary>
        /// Gets the number of text lines in the control
        /// </summary>
        public int GetLinesCount()
        {
            Document.GetText(TextGetOptions.None, out String text);
            return text.Split('\r').Length;
        }

        /// <summary>
        /// Scrolls the control to make the current selection visible, if needed
        /// </summary>
        public void TryScrollToSelection()
        {
            Document.Selection.GetRect(PointOptions.Transform, out Rect rect, out _);
            double
                viewport = _TemplateScrollViewer.ViewportHeight - ScrollBarMargin.Top, // The current visible area, excluding additional padding
                current = rect.Top - _TemplateScrollViewer.VerticalOffset; // The actual transformed Y position, considering the scrolling
            if (current < 0) _TemplateScrollViewer.ChangeView(null, _TemplateScrollViewer.VerticalOffset + current, null, false);
            else if (current > viewport) _TemplateScrollViewer.ChangeView(null, _TemplateScrollViewer.VerticalOffset + (current - viewport) + 32, null, false);
        }

        /// <summary>
        /// Resets the current undo stack so that pressing Ctrl + Z can no longer restore previous states
        /// </summary>
        public void ResetUndoStack()
        {
            uint depth = Document.UndoLimit;
            Document.UndoLimit = 0;
            Document.UndoLimit = depth;
        }
    }
}
