using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A simple converter that returns the <see cref="Visibility"/> value opposite to the input value
    /// </summary>
    public class BoolToInvertedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.To<bool>() ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
