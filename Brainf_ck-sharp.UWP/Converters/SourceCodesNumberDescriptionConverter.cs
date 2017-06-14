using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A converter that returns a description for the number of source codes in the bound collection
    /// </summary>
    public class SourceCodesNumberDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int count = value.To<int>();
            return count > 1
                ? $"{count} {LocalizationManager.GetResource("MoreCodes")}"
                : LocalizationManager.GetResource("OneCode");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
