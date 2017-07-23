using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels.Settings;

namespace Brainf_ck_sharp_UWP.DataModels.Settings
{
    public class CategorizedSettingsViewModel
    {
        public SettingsSectionType SectionType { get; }

        public SettingsViewModel ViewModel { get; }

        public CategorizedSettingsViewModel(SettingsSectionType type, SettingsViewModel viewModel)
        {
            SectionType = type;
            ViewModel = viewModel;
        }
    }
}
