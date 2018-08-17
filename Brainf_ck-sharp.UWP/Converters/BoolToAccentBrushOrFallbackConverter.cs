using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.UI;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A converter that returns either the accent color if the value is true, or a fallback brush
    /// </summary>
    public class BoolToAccentBrushOrFallbackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.To<bool>()
                ? XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("SystemControlHighlightAccentBrush")
                : parameter.To<string>().ToColor().ToBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
