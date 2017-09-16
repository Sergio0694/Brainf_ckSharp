using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters.UserGuide
{
    /// <summary>
    /// A converter for the different sections in the user guide, in uppercase
    /// </summary>
    public class UserGuideSectionTitleUppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return UserGuideSectionTitleConverter.Convert(value.To<UserGuideSection>()).ToUpperInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
