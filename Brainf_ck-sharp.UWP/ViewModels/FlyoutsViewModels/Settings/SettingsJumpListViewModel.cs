using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.Settings;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels.Settings
{
    public class SettingsJumpListViewModel : JumpListViewModelBase<Tuple<String, int>, CategorizedSettingsViewModel>
    {
        private SettingsViewModel Settings { get; } = new SettingsViewModel();

        protected override Task<IList<JumpListGroup<Tuple<String, int>, CategorizedSettingsViewModel>>> OnLoadGroupsAsync()
        {
            IList<JumpListGroup<Tuple<String, int>, CategorizedSettingsViewModel>> list = new List<JumpListGroup<Tuple<String, int>, CategorizedSettingsViewModel>>
            {
                new JumpListGroup<Tuple<String, int>, CategorizedSettingsViewModel>(Tuple.Create("IDE", 6), 
                    new[] { new CategorizedSettingsViewModel(SettingsSectionType.IDE, Settings) }),
                new JumpListGroup<Tuple<String, int>, CategorizedSettingsViewModel>(Tuple.Create("UI", 2), 
                    new[] { new CategorizedSettingsViewModel(SettingsSectionType.UI, Settings) })
            };
            return Task.FromResult(list);
        }
    }
}
