using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.Settings;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels.Settings
{
    public class SettingsJumpListViewModel : JumpListViewModelBase<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>
    {
        private SettingsViewModel Settings { get; } = new SettingsViewModel();

        protected override Task<IList<JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>>> OnLoadGroupsAsync()
        {
            IList<JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>> list = new List<JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>>
            {
                new JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>(Tuple.Create(SettingsSectionType.IDE, 7), 
                    new[] { new CategorizedSettingsViewModel(SettingsSectionType.IDE, Settings) }),
                new JumpListGroup<Tuple<SettingsSectionType, int>, CategorizedSettingsViewModel>(Tuple.Create(SettingsSectionType.UI, 2), 
                    new[] { new CategorizedSettingsViewModel(SettingsSectionType.UI, Settings) })
            };
            return Task.FromResult(list);
        }
    }
}
