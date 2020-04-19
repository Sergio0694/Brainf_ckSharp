using System;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace Brainf_ckSharp.Uwp.Models.Ide
{
    /// <summary>
    /// A model that represents a source code
    /// </summary>
    public sealed class SourceCode : ObservableObject
    {
        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance with the specified parameters
        /// </summary>
        /// <param name="content">The content of the current source code</param>
        /// <param name="file">The <see cref="StorageFile"/> instance for the current source code</param>
        /// <param name="metadata">The metadata for the current file</param>
        private SourceCode(string content, StorageFile? file, CodeMetadata metadata)
        {
            _Content = content;
            File = file;
            Metadata = metadata;
        }

        private string _Content;

        /// <summary>
        /// Gets or sets the content of the current source code
        /// </summary>
        public string Content
        {
            get => _Content;
            set => Set(ref _Content, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="StorageFile"/> instance for the current source code
        /// </summary>
        public StorageFile? File { get; private set; }

        /// <summary>
        /// Gets the associated <see cref="CodeMetadata"/> instance for the current entry
        /// </summary>
        public CodeMetadata Metadata { get; }

        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance with no linked file
        /// </summary>
        /// <returns>A new, empty <see cref="SourceCode"/> instance</returns>
        [Pure]
        public static SourceCode CreateEmpty() => new SourceCode("\r", null, new CodeMetadata());

        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance from the specified reference file
        /// </summary>
        /// <param name="file">The file to read the contents of the source code from</param>
        /// <returns>A new <see cref="SourceCode"/> instance with the contents of <paramref name="file"/></returns>
        [Pure]
        public static async Task<SourceCode> LoadFromReferenceFileAsync(StorageFile file)
        {
            string text = await FileIO.ReadTextAsync(file);

            return new SourceCode(text, null, CodeMetadata.Default);
        }

        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance from the specified editable file
        /// </summary>
        /// <param name="file">The file to read the contents of the source code from</param>
        /// <returns>A new <see cref="SourceCode"/> instance with the contents of <paramref name="file"/></returns>
        [Pure]
        public static async Task<SourceCode?> TryLoadFromEditableFileAsync(StorageFile file)
        {
            try
            {
                string text = await FileIO.ReadTextAsync(file);

                SourceCode code = new SourceCode(text, file, new CodeMetadata());

                string metadata = JsonSerializer.Serialize(code.Metadata);
                StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace(file.GetId(), file, metadata);

                return code;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to save the current data to a specified file
        /// </summary>
        /// <returns><see langword="true"/> if the data was saved successfully, <see langword="false"/> otherwise</returns>
        public async Task<bool> TrySaveAsync()
        {
            Guard.IsNotNull(File, nameof(File));

            try
            {
                await FileIO.WriteTextAsync(File, Content);

                string metadata = JsonSerializer.Serialize(Metadata);
                StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace(File!.GetId(), File, metadata);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to save the current data to a new file
        /// </summary>
        /// <param name="file">The target file to use to save the data</param>
        /// <returns><see langword="true"/> if the data was saved successfully, <see langword="false"/> otherwise</returns>
        public Task<bool> TrySaveAsAsync(StorageFile file)
        {
            File = file;

            return TrySaveAsync();
        }
    }
}
