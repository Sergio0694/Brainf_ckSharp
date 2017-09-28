using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels.Settings;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A converter that returns the title of the input settings section
    /// </summary>
    public class SettingsSectionTypeToUppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SettingsSectionType type = value.To<SettingsSectionType>();
            switch (type)
            {
                case SettingsSectionType.IDE: return "IDE";
                case SettingsSectionType.UI: return "UI";
                case SettingsSectionType.Interpreter: return LocalizationManager.GetResource("Interpreter").ToUpperInvariant();
                default: throw new ArgumentOutOfRangeException("Invalid settings section type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
