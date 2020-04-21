using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;

namespace Brainf_ckSharp.Uwp.Services.Files
{
    /// <summary>
    /// A <see langword="class"/> implementing a UWP version of <see cref="IFile"/>.
    /// </summary>
    public sealed class File : IFile
    {
        /// <summary>
        /// The underlying <see cref="StorageFile"/> instance in use.
        /// </summary>
        private readonly StorageFile file;

        /// <summary>
        /// Creates a new <see cref="StorageFile"/> instance.
        /// </summary>
        /// <param name="file">The input <see cref="StorageFile"/> instance to wrap.</param>
        public File(StorageFile file)
        {
            this.file = file;
        }

        /// <inheritdoc/>
        public string Path => file.Path;

        /// <inheritdoc/>
        public bool IsReadOnly => file.IsFromPackageDirectory();

        /// <inheritdoc/>
        public async Task<(ulong, DateTimeOffset)> GetPropertiesAsync()
        {
            BasicProperties properties = await file.GetBasicPropertiesAsync();

            return (properties.Size, properties.DateModified);
        }

        /// <inheritdoc/>
        public Task<Stream> OpenStreamForReadAsync()
        {
            return file.OpenStreamForReadAsync();
        }

        /// <inheritdoc/>
        public Task<Stream> OpenStreamForWriteAsync()
        {
            return file.OpenStreamForWriteAsync();
        }

        /// <inheritdoc/>
        public void RequestFutureAccessPermission(string metadata)
        {
            string token = file.GetId();

            StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace(token, file, metadata);
        }
    }
}
