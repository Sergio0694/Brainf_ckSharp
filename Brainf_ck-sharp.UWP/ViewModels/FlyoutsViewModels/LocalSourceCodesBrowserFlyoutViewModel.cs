using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Brainf_ck_sharp;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class LocalSourceCodesBrowserFlyoutViewModel : JumpListViewModelBase<SavedSourceCodeType, CategorizedSourceCodeWithSyntaxInfo>
    {
        protected override async Task<IList<JumpListGroup<SavedSourceCodeType, CategorizedSourceCodeWithSyntaxInfo>>> OnLoadGroupsAsync()
        {
            IList<GroupedSourceCodesCategory> categories = await SQLiteManager.Instance.LoadSavedCodesAsync();
            return await Task.Run(() =>
                (from category in categories
                 where category.Items.Count > 0
                 let items =
                 from code in category.Items
                 select new CategorizedSourceCodeWithSyntaxInfo(category.Type, code)
                 select new JumpListGroup<SavedSourceCodeType, CategorizedSourceCodeWithSyntaxInfo>(category.Type, items)).ToArray());
        }

        /// <summary>
        /// Toggles the favorite status for a given code and refreshes the UI
        /// </summary>
        /// <param name="code">The code to edit</param>
        public async Task ToggleFavorite([NotNull] SourceCode code)
        {
            // Update the item in the database
            await SQLiteManager.Instance.ToggleFavoriteStatusAsync(code);
            JumpListGroup<SavedSourceCodeType, CategorizedSourceCodeWithSyntaxInfo>
                favorites = Source.FirstOrDefault(group => group.Key == SavedSourceCodeType.Favorite),
                original = Source.FirstOrDefault(group => group.Key == SavedSourceCodeType.Original);

            // Update the UI
            if (code.Favorited)
            {
                // The item has been favorited, move from default to first section
                if (favorites == null)
                {
                    favorites = new JumpListGroup<SavedSourceCodeType, CategorizedSourceCodeWithSyntaxInfo>(
                        SavedSourceCodeType.Favorite, new[] { new CategorizedSourceCodeWithSyntaxInfo(SavedSourceCodeType.Favorite, code) });
                    Source.Insert(0, favorites);
                }
                else
                {
                    favorites.AddSorted(new CategorizedSourceCodeWithSyntaxInfo(SavedSourceCodeType.Favorite, code), tuple => tuple.Code.Title);
                }

                // Remove from the previous section
                if (original.Any(entry => entry.Code != code))
                    original.Remove(original.First(entry => entry.Code == code));
                else Source.Remove(original);
            }
            else
            {
                // The item has been unfavorited
                if (original == null)
                {
                    original = new JumpListGroup<SavedSourceCodeType, CategorizedSourceCodeWithSyntaxInfo>(
                        SavedSourceCodeType.Original, new[] { new CategorizedSourceCodeWithSyntaxInfo(SavedSourceCodeType.Original, code) });
                    Source.Insert(favorites.Any(entry => entry.Code != code) ? 1 : 0, original);
                }
                else
                {
                    original.AddSorted(new CategorizedSourceCodeWithSyntaxInfo(SavedSourceCodeType.Original, code), tuple => tuple.Code.Title);
                }

                // Remove from the previous section
                if (favorites.Any(entry => entry.Code != code))
                    favorites.Remove(favorites.First(entry => entry.Code == code));
                else Source.Remove(favorites);
            }
        }

        /// <summary>
        /// Deletes a saved code from the database
        /// </summary>
        /// <param name="code">The saved code to delete</param>
        public async Task DeleteItemAsync([NotNull] SourceCode code)
        {
            // Delete the code from the database
            await SQLiteManager.Instance.DeleteCodeAsync(code);

            // Update the UI
            JumpListGroup<SavedSourceCodeType, CategorizedSourceCodeWithSyntaxInfo> section = Source.FirstOrDefault(
                group => group.Key == (code.Favorited ? SavedSourceCodeType.Favorite : SavedSourceCodeType.Original));
            if (section.Any(entry => entry.Code != code))
                section.Remove(section.First(entry => entry.Code == code));
            else Source.Remove(section);
        }

        /// <summary>
        /// Shared a saved source code
        /// </summary>
        /// <param name="mode">The desired share mode</param>
        /// <param name="code">The code to share</param>
        public async Task<AsyncOperationResult<bool>> ShareItemAsync(SourceCodeShareType mode, [NotNull] SourceCode code)
        {
            switch (mode)
            {
                case SourceCodeShareType.Clipboard:
                    
                    // Try to perform the copy operation
                    try
                    {
                        DataPackage package = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
                        package.SetText(code.Code);
                        Clipboard.SetContent(package);
                        return true;
                    }
                    catch
                    {
                        // Whops!
                        return false;
                    }
                case SourceCodeShareType.OSShare:
                    ShareCharmsHelper.ShareText(code.Title, code.Code);
                    return true;
                case SourceCodeShareType.Email:
                    StorageFile file = await StorageHelper.CreateTemporaryFileAsync(code.Title, ".txt");
                    if (file == null) return AsyncOperationStatus.Canceled;
                    await FileIO.WriteTextAsync(file, code.Code);
                    return await EmailHelper.SendEmail(String.Empty, LocalizationManager.GetResource("SharedCode"), null, file);
                case SourceCodeShareType.LocalFile:
                    StorageFile local = await StorageHelper.PickSaveFileAsync(code.Title, LocalizationManager.GetResource("PlainText"), ".txt");
                    if (local == null) return AsyncOperationStatus.Canceled;
                    await FileIO.WriteTextAsync(local, code.Code);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        /// <summary>
        /// Exports a C translation of the given code
        /// </summary>
        /// <param name="code">The input code to translate</param>
        public async Task<AsyncOperationResult<bool>> ExportToCAsync([NotNull] SourceCode code)
        {
            Messenger.Default.Send(new AppLoadingStatusChangedMessage(true));
            StorageFile local = await StorageHelper.PickSaveFileAsync(code.Title, LocalizationManager.GetResource("CSource"), ".c");
            if (local == null)
            {
                Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
                return AsyncOperationStatus.Canceled;
            }
            String translation = await Task.Run(() => Brainf_ckInterpreter.TranslateToC(code.Code));
            await FileIO.WriteTextAsync(local, translation);
            Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
            return true;
        }

        /// <summary>
        /// Renames a source code
        /// </summary>
        /// <param name="code">The code to edit</param>
        /// <param name="title">The new title to assign to the code</param>
        public async Task RenameItemAsync([NotNull] SourceCode code, [NotNull] String title)
        {
            // Update the item in the database
            await SQLiteManager.Instance.RenameCodeAsync(code, title);

            // Ensure the items in the edited section are sorted
            JumpListGroup<SavedSourceCodeType, CategorizedSourceCodeWithSyntaxInfo> section = Source.FirstOrDefault(
                group => group.Key == (code.Favorited ? SavedSourceCodeType.Favorite : SavedSourceCodeType.Original));
            CategorizedSourceCodeWithSyntaxInfo item = section.First(entry => entry.Code == code);
            if (!section.EnsureSorted(item, entry => entry.Code.Title))
            {
                // Force an UI refresh
                int index = section.IndexOf(item);
                section.Remove(item);
                section.Insert(index, item);
            }
        }
    }
}
