using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp.Legacy.UWP.Enums;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;

namespace Brainf_ck_sharp.Legacy.UWP.Converters.UserGuide
{
    /// <summary>
    /// A simple converter for the description of the user guide sections
    /// </summary>
    public class UserGuideSectionDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value.To<UserGuideSection>())
            {
                case UserGuideSection.Introduction: return LocalizationManager.GetResource("GuideIntroductionDescription");
                case UserGuideSection.Samples: return LocalizationManager.GetResource("GuideCodeSamplesDescription");
                case UserGuideSection.PBrain: return LocalizationManager.GetResource("GuidePBrainDescription");
                case UserGuideSection.Debugging: return LocalizationManager.GetResource("DebuggingDescription");
                case UserGuideSection.KeyboardShortcuts: return LocalizationManager.GetResource("KeyboardShortcutsDescription");
                default: return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
