using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Brainf_ckSharp.Services;

#nullable enable

namespace Brainf_ckSharp.Uwp.Services.Files
{
    /// <summary>
    /// A <see langword="class"/> that implements the <see cref="IFilesService"/> <see langword="interface"/> using UWP APIs
    /// </summary>
    public sealed class FilesService : IFilesService
    {
        /// <inheritdoc/>
        public string InstallationPath => Package.Current.InstalledLocation.Path;

        /// <inheritdoc/>
        public string TemporaryFilesPath => ApplicationData.Current.TemporaryFolder.Path;

        /// <inheritdoc/>
        public async Task<IFile> GetFileFromPathAsync(string path)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(path);

            return new File(file);
        }

        /// <inheritdoc/>
        public async Task<IFile?> TryGetFileFromPathAsync(string path)
        {
            try
            {
                return await GetFileFromPathAsync(path);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<IFile> CreateOrOpenFileFromPathAsync(string path)
        {
            string
                folderPath = Path.GetDirectoryName(path),
                filename = Path.GetFileName(path);

            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);

            StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

            return new File(file);
        }

        /// <inheritdoc/>
        public async Task<IFile?> TryPickOpenFileAsync(string extension)
        {
            // Create the file picker
            FileOpenPicker picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            // Add the given extensions to look for
            picker.FileTypeFilter.Add(extension);

            if (await picker.PickSingleFileAsync() is StorageFile file)
            {
                return new File(file);
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<IFile?> TryPickSaveFileAsync(string filename, (string Name, string Extension) fileType)
        {
            // Create the file picker
            FileSavePicker picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = fileType.Extension,
                SuggestedFileName = filename
            };

            // Add the extensions and pick the file
            picker.FileTypeChoices.Add(fileType.Name, new[] { fileType.Extension });

            if (await picker.PickSaveFileAsync() is StorageFile file)
            {
                return new File(file);
            }

            return null;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<(IFile, string)> GetFutureAccessFilesAsync()
        {
            IReadOnlyList<AccessListEntry> entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries.ToArray();

            foreach (var entry in entries)
            {
                StorageFile file;

                // Try to get the target file
                try
                {
                    file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(entry.Token);
                }
                catch (FileNotFoundException)
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Remove(entry.Token);

                    continue;
                }

                yield return (new File(file), entry.Metadata);
            }
        }
    }
}
