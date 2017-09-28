using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.TemplateSelectors
{
    /// <summary>
    /// A template selector for the user guide sections
    /// </summary>
    class UserGuideSectionTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent)
            {
                switch (item.To<UserGuideSection>())
                {
                    case UserGuideSection.Introduction: return parent.FindResource<DataTemplate>("IntroductionSectionTemplate");
                    case UserGuideSection.Samples: return parent.FindResource<DataTemplate>("CodeSamplesectionTemplate");
                    case UserGuideSection.PBrain: return parent.FindResource<DataTemplate>("PBrainSectionTemplate");
                    default: throw new ArgumentOutOfRangeException("Invalid user guide section");
                }
            }
            return null;
        }
    }
}
