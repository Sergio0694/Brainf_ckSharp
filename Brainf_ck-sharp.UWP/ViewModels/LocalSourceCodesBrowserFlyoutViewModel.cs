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

        public async void ToggleFavorite(SourceCode code)
        {
            await SQLiteManager.Instance.ToggleFavoriteStatusAsync(code);
            JumpListGroup<SavedSourceCodeType, Tuple<SavedSourceCodeType, SourceCode>>
                favorites = Source.FirstOrDefault(group => group.Key == SavedSourceCodeType.Favorite),
                original = Source.FirstOrDefault(group => group.Key == SavedSourceCodeType.Original);

            if (code.Favorited)
            {
                // The item has been favorited, move from default to first section
                if (favorites == null)
                {
                    favorites = new JumpListGroup<SavedSourceCodeType, Tuple<SavedSourceCodeType, SourceCode>>(
                        SavedSourceCodeType.Favorite, new[] { Tuple.Create(SavedSourceCodeType.Favorite, code) });
                    Source.Insert(0, favorites);
                }
                else
                {
                    favorites.Add(Tuple.Create(SavedSourceCodeType.Favorite, code));
                }

                // Remove from the previous section
                if (original.Any(entry => entry.Item2 != code))
                    original.Remove(original.First(entry => entry.Item2 == code));
                else Source.Remove(original);
            }
            else
            {
                if (original == null)
                {
                    original = new JumpListGroup<SavedSourceCodeType, Tuple<SavedSourceCodeType, SourceCode>>(
                        SavedSourceCodeType.Original, new[] { Tuple.Create(SavedSourceCodeType.Original, code) });
                    Source.Insert(favorites.Any(entry => entry.Item2 != code) ? 1 : 0, original);
                }
                else
                {
                    original.Add(Tuple.Create(SavedSourceCodeType.Original, code));
                }

                // Remove from the previous section
                if (favorites.Any(entry => entry.Item2 != code))
                    favorites.Remove(favorites.First(entry => entry.Item2 == code));
                else Source.Remove(favorites);
            }
        }
    }
}
