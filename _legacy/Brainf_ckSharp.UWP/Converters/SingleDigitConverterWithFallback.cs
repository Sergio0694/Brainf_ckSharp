using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.Converters
{
    /// <summary>
    /// A converter that returns a single digit value or a fallback string
    /// </summary>
    public class SingleDigitConverterWithFallback : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.To<uint>() < 10 ? value.ToString() : (parameter.To<string>() ?? string.Empty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
