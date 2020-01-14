using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Brainf_ckSharp.Uwp.Services.Files
{
    /// <summary>
    /// A <see langword="class"/> that implements the <see cref="IFilesService"/> <see langword="interface"/> using UWP APIs
    /// </summary>
    public sealed class FilesService : IFilesService
    {
        /// <inheritdoc/>
        public Task<StorageFile> TryPickOpenFileAsync(string extension)
        {
            // Create the file picker
            FileOpenPicker picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            // Add the given extensions to look for
            picker.FileTypeFilter.Add(extension);

            return picker.PickSingleFileAsync().AsTask();
        }

        /// <inheritdoc/>
        public Task<StorageFile> TryPickSaveFileAsync(string filename, (string Name, string Extension) fileType)
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

            return picker.PickSaveFileAsync().AsTask();
        }
    }
}
