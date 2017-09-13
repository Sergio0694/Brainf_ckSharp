using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A <see cref="Panel"/> that displays and arranges items of variable width
    /// </summary>
    public class WrapPanel : Panel
    {
        /// <summary>
        /// Gets or sets the height of the items in the panel
        /// </summary>
        public double ItemHeight
        {
            get => GetValue(ItemHeightProperty).To<double>();
            set => SetValue(ItemHeightProperty, value);
        }

        public static DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight), typeof(double), typeof(WrapPanel), new PropertyMetadata(double.NaN, OnItemHeightChanged));

        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<WrapPanel>().InvalidateMeasure();
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double
                x = 0,
                y = 0,
                maxX = 0,
                rowHeight = double.IsNaN(ItemHeight) ? 0 : ItemHeight;
            foreach (UIElement c in Children)
            {
                Size desired = c.DesiredSize;
                double rightSide = x + desired.Width;
                if (rightSide >= finalSize.Width + 0.5)
                {
                    x = 0;
                    y += rowHeight;
                }
                c.Arrange(new Rect(x, y, desired.Width, double.IsNaN(ItemHeight) ? desired.Height : ItemHeight));
                if (double.IsNaN(ItemHeight) && desired.Height > rowHeight) rowHeight = desired.Height;
                x += desired.Width;
                if (x > maxX) maxX = x;
            }
            return finalSize;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            double
                x = 0,
                y = 0,
                maxX = 0,
                rowHeight = double.IsNaN(ItemHeight) ? 0 : ItemHeight;
            foreach (UIElement c in Children)
            {
                c.Measure(new Size(availableSize.Width, double.IsNaN(ItemHeight) ? availableSize.Height : ItemHeight));
                Size ds = c.DesiredSize;
                if (double.IsNaN(ItemHeight) && ds.Height > rowHeight) rowHeight = ds.Height;
                x += ds.Width;
                if (x >= availableSize.Width)
                {
                    x = ds.Width;
                    y += rowHeight;
                }
                if (x > maxX) maxX = x;
            }
            return new Size(maxX, y + rowHeight);
        }
    }
}
