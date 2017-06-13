using System;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A RichEditBox that provides info on its inner ScrollViewer control
    /// </summary>
    public class RichEditBoxWithVerticalOffsetInfo : RichEditBox
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _TemplateScrollViewer = GetTemplateChild("ContentElement") as ScrollViewer;
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
    }
}
