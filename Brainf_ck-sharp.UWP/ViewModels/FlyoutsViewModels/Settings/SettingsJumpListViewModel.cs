using System;
using System.Collections.Generic;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.Settings;
using Brainf_ck_sharp_UWP.ViewModels.Abstract.JumpList;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels.Settings
{
    public class SettingsJumpListViewModel : SyncJumpListViewModelBase<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>
    {
        /// <summary>
        /// Gets the shared <see cref="SettingsViewModel"/> instance for all the settings sections
        /// </summary>
        private SettingsViewModel Settings { get; } = new SettingsViewModel();

        protected override IList<JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>> OnLoadGroups()
        {
            return new List<JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>>
            {
                new JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>(Tuple.Create(SettingsSectionType.IDE, 7), 
                    new[] { new CategorizedSettingsViewModel(SettingsSectionType.IDE, Settings) }),
                new JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>(Tuple.Create(SettingsSectionType.UI, 2), 
                    new[] { new CategorizedSettingsViewModel(SettingsSectionType.UI, Settings) })
            };
        }
    }
}
