using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.AttachedProperties
{
    /// <summary>
    /// An attached property that creates an angle in the top left corner of the input polygon
    /// </summary>
    public class CustomPolygonBorderAngleHelper
    {
        [UsedImplicitly] // XAML attached property
        public static int GetCustomPolygonBorderAngle(Polygon element)
        {
            return element.GetValue(CustomPolygonBorderAngleProperty).To<int>();
        }

        public static void SetCustomPolygonBorderAngle(Polygon element, int value)
        {
            element?.SetValue(CustomPolygonBorderAngleProperty, value);
        }

        public static readonly DependencyProperty CustomPolygonBorderAngleProperty =
            DependencyProperty.RegisterAttached("CustomPolygonBorderAngle", typeof(int), typeof(CustomPolygonBorderAngleHelper), 
                new PropertyMetadata(DependencyProperty.UnsetValue, OnCustomPolygonBorderAnglePropertyChanged));

        private static void OnCustomPolygonBorderAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is int value)
            {
                d.To<Polygon>().Points = new PointCollection
                {
                    new Point(value / 2d, 0),
                    new Point(value, 0),
                    new Point(value, value / 2d)
                };
            }
        }
    }
}
