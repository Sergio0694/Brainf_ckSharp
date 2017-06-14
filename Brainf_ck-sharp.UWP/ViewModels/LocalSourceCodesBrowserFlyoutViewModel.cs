using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class LocalSourceCodesBrowserFlyoutViewModel : JumpListViewModelBase<SavedSourceCodeType, SourceCode>
    {
        protected override async Task<IList<JumpListGroup<SavedSourceCodeType, SourceCode>>> OnLoadGroupsAsync()
        {
            return (from category in await SQLiteManager.Instance.LoadSavedCodesAsync()
                    where category.Items.Count > 0
                    select new JumpListGroup<SavedSourceCodeType, SourceCode>(category.Type, category.Items)).ToArray();
        }
    }
}
