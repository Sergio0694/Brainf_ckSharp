using System.Collections.Generic;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.Settings;
using Brainf_ck_sharp_UWP.ViewModels.Abstract.JumpList;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels.Settings
{
    public class SettingsJumpListViewModel : SyncJumpListViewModelBase<CategorizedSettingsViewModel, CategorizedSettingsViewModel>
    {
        /// <summary>
        /// Gets the shared <see cref="SettingsViewModel"/> instance for all the settings sections
        /// </summary>
        private SettingsViewModel Settings { get; } = new SettingsViewModel();

        /* ===================
         * NOTE
         * ===================
         * As in other parts of the app, the APIs to handle the SemanticZoom control and its data are quite messy,
         * therefore the following method doesn't seem to make much sense on its own */
        protected override IList<JumpListGroup<CategorizedSettingsViewModel, CategorizedSettingsViewModel>> OnLoadGroups()
        {
            // Helper function to setup a group with a single categorized instance
            JumpListGroup<CategorizedSettingsViewModel, CategorizedSettingsViewModel> SpawnGroup(SettingsSectionType type)
            {
                CategorizedSettingsViewModel reference = new CategorizedSettingsViewModel(type, Settings);
                return new JumpListGroup<CategorizedSettingsViewModel, CategorizedSettingsViewModel>(reference, new[] { reference });
            }

            // Create and return the sections list
            return new List<JumpListGroup<CategorizedSettingsViewModel, CategorizedSettingsViewModel>>
            {
                SpawnGroup(SettingsSectionType.IDE),
                SpawnGroup(SettingsSectionType.UI),
                SpawnGroup(SettingsSectionType.Interpreter)
            };
        }
    }
}
