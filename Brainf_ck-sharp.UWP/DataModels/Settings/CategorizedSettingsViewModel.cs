using System;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels.Settings;
using JetBrains.Annotations;

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
        [NotNull]
        public SettingsViewModel ViewModel { get; }

        /// <summary>
        /// Creates a new instance for a settings section
        /// </summary>
        /// <param name="type">The settings section type</param>
        /// <param name="viewModel">The shared <see cref="SettingsViewModel"/> instance</param>
        public CategorizedSettingsViewModel(SettingsSectionType type, [NotNull] SettingsViewModel viewModel)
        {
            SectionType = type;
            ViewModel = viewModel;
        }

        /// <summary>
        /// Gets a small description of the section contents
        /// </summary>
        [NotNull]
        public String SectionDescription
        {
            get
            {
                switch (SectionType)
                {
                    case SettingsSectionType.IDE:
                        return ViewModel.ThemesSelectorEnabled
                            ? $"7 {LocalizationManager.GetResource("LowercaseAvailableSettings")}"
                            : $"6 {LocalizationManager.GetResource("LowercaseAvailableSettings")}, {LocalizationManager.GetResource("ThemesPackLocked")}";
                    case SettingsSectionType.UI:
                        return $"1 {LocalizationManager.GetResource("LowercaseSingleAvailableSettings")}";
                    default:
                        throw new ArgumentOutOfRangeException("Invalid section type");
                }
            }
        }
    }
}
