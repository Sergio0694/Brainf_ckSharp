using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.InheritedControls
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
                if (_TemplateScrollBar == null) throw new NullReferenceException("Invalid template");
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
            _TemplateScrollViewer = GetTemplateChild("ContentScroller").To<ScrollViewer>();
            _TextPresenter = GetTemplateChild("ContentElement").To<ContentPresenter>();
            _TextPresenter.SizeChanged += _TextPresenter_SizeChanged;
            if (_TemplateScrollViewer == null || _TextPresenter == null) throw new NullReferenceException("Invalid template content");
        }

        /// <summary>
        /// Raised whenever the size of the inner text changes
        /// </summary>
        public event SizeChangedEventHandler TextSizeChanged;

        private void _TextPresenter_SizeChanged(object sender, SizeChangedEventArgs e) => TextSizeChanged?.Invoke(sender, e);

        /// <summary>
        /// Gets the inner <see cref="ScrollViewer"/>, once the control has been added to the visual tree and loaded
        /// </summary>
        private ScrollViewer _TemplateScrollViewer;

        /// <summary>
        /// Gets the inner <see cref="ContentPresenter"/>, once the control has been added to the visual tree and loaded
        /// </summary>
        private ContentPresenter _TextPresenter;

        /// <summary>
        /// Gets the inner <see cref="ScrollViewer"/> inside the control
        /// </summary>
        public ScrollViewer InnerScrollViewer => _TemplateScrollViewer;

        /// <summary>
        /// Gets the curent vertical offset of the inner ScrollViewer
        /// </summary>
        public double VerticalScrollViewerOffset => _TemplateScrollViewer.VerticalOffset;

        /// <summary>
        /// Gets the actual vertical offset of the current text selection
        /// </summary>
        public Point ActualSelectionVerticalOffset
        {
            get
            {
                Document.Selection.GetRect(PointOptions.Transform, out Rect textRect, out _);
                return new Point(textRect.X, textRect.Top);
            }
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
            double? horizontal;
            if (rect.Left < _TemplateScrollViewer.HorizontalOffset)
                horizontal = rect.Left - 12;
            else if (rect.Left > _TemplateScrollViewer.ViewportWidth + _TemplateScrollViewer.HorizontalOffset - 20)
                horizontal = rect.Left - _TemplateScrollViewer.ViewportWidth + 32;
            else horizontal = null;
            if (current < 0) _TemplateScrollViewer.ChangeView(horizontal, _TemplateScrollViewer.VerticalOffset + current, null, false);
            else if (current > viewport) _TemplateScrollViewer.ChangeView(horizontal, _TemplateScrollViewer.VerticalOffset + (current - viewport) + 32, null, false);
            else _TemplateScrollViewer.ChangeView(horizontal, null, null, false);
        }

        /// <summary>
        /// Resets the current undo stack so that pressing Ctrl + Z can no longer restore previous states
        /// </summary>
        public void ResetTextAndUndoStack()
        {
            try
            {
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    Document.LoadFromStream(TextSetOptions.None, stream);
                }
            }
            catch
            {
                // Whops!
            }
        }

        /// <summary>
        /// Loads a text into the current document and resets the undo stack
        /// </summary>
        /// <param name="text">The input text to load</param>
        public async Task LoadTextAsync([NotNull] string text)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                byte[] bytes = Encoding.Unicode.GetBytes(text);
                await stream.WriteAsync(bytes.AsBuffer());
                Document.LoadFromStream(TextSetOptions.None, stream);
            }
        }

        /// <summary>
        /// Sets the default tab spacing value for the document in use
        /// </summary>
        /// <param name="length">The desired tab spacing value</param>
        public void SetTabLength(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length), "Invalid length value");
            Document.DefaultTabStop = length * 3; // Each space has an approximate width of 3 points
            ITextParagraphFormat format = Document.GetDefaultParagraphFormat();
            format.ClearAllTabs();
            Document.SetDefaultParagraphFormat(format);
        }

        /// <summary>
        /// Sets the font for the text displayed in the control
        /// </summary>
        /// <param name="name">The name of the new font to use</param>
        /// <remarks>The input name is not validated and should be checked before calling this method</remarks>
        public void SetFontFamily([NotNull] string name)
        {
            ITextCharacterFormat format = Document.GetDefaultCharacterFormat();
            format.Name = name;
            Document.SetDefaultCharacterFormat(format);
            Document.GetRange(0, int.MaxValue).CharacterFormat.Name = name;
        }

        // The list of shortcut keys to ignore
        [NotNull]
        private readonly HashSet<VirtualKey> SkippedShortcuts = new HashSet<VirtualKey>(new[] { VirtualKey.E, VirtualKey.R, VirtualKey.J, VirtualKey.L });

        /// <inheritdoc cref="RichEditBox"/>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if ((Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down && SkippedShortcuts.Contains(e.Key))
            {
                return;
            }
            base.OnKeyDown(e);
        }
    }
}
