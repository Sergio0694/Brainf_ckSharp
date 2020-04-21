using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

#nullable enable

namespace Brainf_ckSharp.Uwp.Services.Files
{
    /// <summary>
    /// The default <see langword="interface"/> for a service that handles files
    /// </summary>
    public interface IFilesService
    {
        /// <summary>
        /// Tries to pick a file to open with a specified extension
        /// </summary>
        /// <param name="extension">The extension to use, in the format ".{extension}"</param>
        /// <returns>A <see cref="StorageFile"/> to open, if available</returns>
        Task<StorageFile?> TryPickOpenFileAsync(string extension);

        /// <summary>
        /// Tries to pick a file to save to with the specified parameters
        /// </summary>
        /// <param name="filename">The suggested filename to use</param>
        /// <param name="fileType">The info on the file type to save to</param>
        /// <returns>A <see cref="StorageFile"/> to use to save data to, if available</returns>
        Task<StorageFile?> TryPickSaveFileAsync(string filename, (string Name, string Extension) fileType);

        /// <summary>
        /// Enumerates all the available files from the future access list.
        /// </summary>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> sequence of available files.</returns>
        IAsyncEnumerable<(IFile File, string Metadata)> GetFutureAccessFilesAsync();
    }
}
