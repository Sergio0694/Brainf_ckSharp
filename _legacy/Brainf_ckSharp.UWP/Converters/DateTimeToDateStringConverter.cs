using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.Converters
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> date into its short date representation
    /// </summary>
    public class DateTimeToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.To<DateTime>().ToString("d");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
