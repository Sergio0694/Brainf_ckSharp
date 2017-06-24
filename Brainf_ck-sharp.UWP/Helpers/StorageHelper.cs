using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A static class with some helper methods to work with local files
    /// </summary>
    public static class StorageHelper
    {
        /// <summary>
        /// Picks an external file from the current device
        /// </summary>
        /// <param name="location">The initial location when looking for a file to pick</param>
        /// <param name="extensions">The desired extensions to filter the results</param>
        [MustUseReturnValue, ItemCanBeNull]
        public static Task<StorageFile> PickFileAsync(PickerLocationId location, params String[] extensions)
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = location
            };
            foreach (String extension in extensions.Where(ext => !String.IsNullOrEmpty(ext)))
            {
                String targetExtension;
                if (!extension.Equals("*") && !extension.StartsWith(".")) targetExtension = $".{extension}";
                else targetExtension = extension;
                picker.FileTypeFilter.Add(targetExtension);
            }
            return picker.PickSingleFileAsync().AsTask();
        }

        /// <summary>
        /// Picks an output file to save data from the app
        /// </summary>
        /// <param name="filename">The desired filename for the new file</param>
        /// <param name="fileType">The descriptive file type</param>
        /// <param name="extension">The desired extension for the file</param>
        [Pure, ItemCanBeNull]
        public static Task<StorageFile> PickSaveFileAsync(String filename, String fileType, String extension)
        {
            String validName = Path.GetInvalidFileNameChars().Where(filename.Contains).Aggregate(filename, (current, c) => current.Replace(c.ToString(), String.Empty));
            if (validName.Length == 0) return null;
            FileSavePicker picker = new FileSavePicker
            {
                DefaultFileExtension = extension,
                SuggestedFileName = validName
            };
            picker.FileTypeChoices.Add(fileType, new List<String> { extension });
            return picker.PickSaveFileAsync().AsTask();
        }

        /// <summary>
        /// Creates and returns a temporary file with the given name and extension
        /// </summary>
        /// <param name="filename">The desired filename for the file</param>
        /// <param name="extension">The file extension to use</param>
        [MustUseReturnValue, ItemCanBeNull]
        public static async Task<StorageFile> CreateTemporaryFileAsync(String filename, String extension)
        {
            String validName = Path.GetInvalidFileNameChars().Where(filename.Contains).Aggregate(filename, (current, c) => current.Replace(c.ToString(), String.Empty));
            if (validName.Length == 0) return null;
            return await ApplicationData.Current.TemporaryFolder.CreateFileAsync($"{validName}{extension}", CreationCollisionOption.ReplaceExisting);
        }
    }
}
