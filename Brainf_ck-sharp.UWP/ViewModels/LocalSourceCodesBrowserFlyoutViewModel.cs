using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class LocalSourceCodesBrowserFlyoutViewModel : JumpListViewModelBase<SavedSourceCodeType, Tuple<SavedSourceCodeType, SourceCode>>
    {
        protected override async Task<IList<JumpListGroup<SavedSourceCodeType, Tuple<SavedSourceCodeType, SourceCode>>>> OnLoadGroupsAsync()
        {
            IList<(SavedSourceCodeType Type, IList<SourceCode> Items)> categories = await SQLiteManager.Instance.LoadSavedCodesAsync();
            return (from category in categories
                    where category.Items.Count > 0
                    let items =
                        from code in category.Items
                        select Tuple.Create(category.Type, code)
                    select new JumpListGroup<SavedSourceCodeType, Tuple<SavedSourceCodeType, SourceCode>>(category.Type, items)).ToArray();
        }
    }
}
