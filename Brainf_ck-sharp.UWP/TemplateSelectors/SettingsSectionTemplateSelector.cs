using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.Settings;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.TemplateSelectors
{
    /// <summary>
    /// A template selector for a settings section
    /// </summary>
    public class SettingsSectionTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent)
            {
                switch (item)
                {
                    case CategorizedSettingsViewModel section when section.SectionType == SettingsSectionType.IDE:
                        return parent.FindResource<DataTemplate>("IDESettingsTemplate");
                    case CategorizedSettingsViewModel section when section.SectionType == SettingsSectionType.UI:
                        return parent.FindResource<DataTemplate>("UISettingsTemplate");
                    case CategorizedSettingsViewModel section when section.SectionType == SettingsSectionType.Interpreter:
                        return parent.FindResource<DataTemplate>("InterpreterSettingsTemplate");
                }
            }
            return null;
        }
    }
}
