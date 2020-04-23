using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

namespace Brainf_ckSharp.Shared.Models
{
    /// <summary>
    /// A simple model that associates a specific section to a <see cref="SettingsSubPageViewModel"/> instance
    /// </summary>
    public sealed class SettingsViewModelWithSectionInfo
    {
        /// <summary>
        /// Creates a new <see cref="SettingsViewModelWithSectionInfo"/> instance with the specified parameters
        /// </summary>
        /// <param name="section">The current section being targeted</param>
        /// <param name="instance">The <see cref="SettingsSubPageViewModel"/> instance to wrap</param>
        public SettingsViewModelWithSectionInfo(SettingsSection section, SettingsSubPageViewModel instance)
        {
            Section = section;
            Instance = instance;
        }

        /// <summary>
        /// Gets the current section being targeted
        /// </summary>
        public SettingsSection Section { get; }

        /// <summary>
        /// Gets the <see cref="SettingsSubPageViewModel"/> instance currently wrapped
        /// </summary>
        public SettingsSubPageViewModel Instance { get; }
    }
}
