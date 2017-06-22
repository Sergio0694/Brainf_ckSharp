using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A simple converter that returns a string representation for a category of saved source codes
    /// </summary>
    public class SourceCodeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value.To<SavedSourceCodeType>());
        }

        public static String Convert(SavedSourceCodeType type)
        {
            switch (type)
            {
                case SavedSourceCodeType.Sample:
                    return LocalizationManager.GetResource("SampleCodes");
                case SavedSourceCodeType.Favorite:
                    return LocalizationManager.GetResource("Favorites");
                case SavedSourceCodeType.Original:
                    return LocalizationManager.GetResource("PersonalCodes");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A simple converter that returns a string representation for a category of saved source codes, in uppercase
    /// </summary>
    public class SourceCodeTypeUppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return SourceCodeTypeConverter.Convert(value.To<SavedSourceCodeType>()).ToUpperInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
