﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

#nullable enable

namespace Brainf_ckSharp.Uwp.Services.Files
{
    /// <summary>
    /// A <see langword="class"/> that implements the <see cref="IFilesService"/> <see langword="interface"/> using UWP APIs
    /// </summary>
    public sealed class FilesService : IFilesService
    {
        /// <inheritdoc/>
        public async Task<IFile> GetFileFromPathAsync(string path)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(path);

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
                // Try to get the target file
                StorageFile? file = await StorageApplicationPermissions.MostRecentlyUsedList.TryGetFileAsync(entry.Token);

                if (file is null)
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Remove(entry.Token);

                    continue;
                }

                yield return (new File(file), entry.Metadata);
            }
        }
    }
}
