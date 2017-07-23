using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels.Settings;

namespace Brainf_ck_sharp_UWP.DataModels.Settings
{
    /// <summary>
    /// Represents a settings section with a reference to the main <see cref="SettingsViewModel"/> instance
    /// </summary>
    public class CategorizedSettingsViewModel
    {
        /// <summary>
        /// Gets the type of the current section
        /// </summary>
        public SettingsSectionType SectionType { get; }

        /// <summary>
        /// Gets the reference to the shared view model
        /// </summary>
        public SettingsViewModel ViewModel { get; }

        /// <summary>
        /// Creates a new instance for a settings section
        /// </summary>
        /// <param name="type">The settings section type</param>
        /// <param name="viewModel">The shared <see cref="SettingsViewModel"/> instance</param>
        public CategorizedSettingsViewModel(SettingsSectionType type, SettingsViewModel viewModel)
        {
            SectionType = type;
            ViewModel = viewModel;
        }
    }
}
