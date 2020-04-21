using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Brainf_ckSharp.Services;
using Microsoft.Toolkit.Extensions;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace Brainf_ckSharp.Uwp.Services.Files
{
    /// <summary>
    /// A <see langword="class"/> implementing a UWP version of <see cref="IFile"/>.
    /// </summary>
    public sealed class File : IFile
    {
        /// <summary>
        /// Creates a new <see cref="Windows.Storage.StorageFile"/> instance.
        /// </summary>
        /// <param name="storageFile">The input <see cref="Windows.Storage.StorageFile"/> instance to wrap.</param>
        public File(StorageFile storageFile)
        {
            StorageFile = storageFile;
        }

        /// <summary>
        /// The underlying <see cref="Windows.Storage.StorageFile"/> instance in use.
        /// </summary>
        internal StorageFile StorageFile { get; }

        /// <inheritdoc/>
        public string DisplayName => StorageFile.DisplayName;

        /// <inheritdoc/>
        public string Path => StorageFile.Path;

        /// <inheritdoc/>
        public bool IsReadOnly => Path.StartsWith(Package.Current.InstalledLocation.Path);

        /// <inheritdoc/>
        public async Task<(ulong, DateTimeOffset)> GetPropertiesAsync()
        {
            BasicProperties properties = await StorageFile.GetBasicPropertiesAsync();

            return (properties.Size, properties.DateModified);
        }

        /// <inheritdoc/>
        public Task<Stream> OpenStreamForReadAsync()
        {
            return StorageFile.OpenStreamForReadAsync();
        }

        /// <inheritdoc/>
        public Task<Stream> OpenStreamForWriteAsync()
        {
            return StorageFile.OpenStreamForWriteAsync();
        }

        /// <inheritdoc/>
        public Task DeleteAsync()
        {
            return StorageFile.DeleteAsync().AsTask();
        }

        /// <inheritdoc/>
        public void RequestFutureAccessPermission(string metadata)
        {
            string token = Path.GetDjb2HashCode().ToHexString();

            StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace(token, StorageFile, metadata);
        }

        /// <inheritdoc/>
        public void RemoveFutureAccessPermission()
        {
            string token = Path.GetDjb2HashCode().ToHexString();

            StorageApplicationPermissions.MostRecentlyUsedList.Remove(token);
        }
    }
}
