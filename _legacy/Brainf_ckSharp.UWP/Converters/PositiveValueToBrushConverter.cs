using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Brainf_ck_sharp.Legacy.UWP.Converters
{
    /// <summary>
    /// A converter that binds to an int value returns the accent color or a fallback color depending on the input value
    /// </summary>
    public class PositiveValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is uint u && u > 0 || value is int i && i > 0
                ? XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("SystemControlHighlightAccentBrush")
                : new SolidColorBrush(parameter.To<string>().ToColor());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
