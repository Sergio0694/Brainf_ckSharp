using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters.UserGuide
{
    /// <summary>
    /// A converter for the different sections in the user guide
    /// </summary>
    public class UserGuideSectionTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value.To<UserGuideSection>());
        }

        /// <summary>
        /// Converts a section to its localized name to display
        /// </summary>
        /// <param name="section">The current section</param>
        public static string Convert(UserGuideSection section)
        {
            switch (section)
            {
                case UserGuideSection.Introduction: return LocalizationManager.GetResource("UserGuideIntroduction");
                case UserGuideSection.Samples: return LocalizationManager.GetResource("UserGuideCodeSamples");
                case UserGuideSection.PBrain: return LocalizationManager.GetResource("UserGuidePBrainExtensions");
                default: return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
