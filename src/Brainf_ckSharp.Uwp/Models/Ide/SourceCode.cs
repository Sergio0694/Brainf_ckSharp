using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

#nullable enable

namespace Brainf_ckSharp.Uwp.Models.Ide
{
    /// <summary>
    /// A model that represents a source code
    /// </summary>
    public sealed class SourceCode : ViewModelBase
    {
        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance with the specified parameters
        /// </summary>
        /// <param name="content">The content of the current source code</param>
        /// <param name="file">The <see cref="StorageFile"/> instance for the current source code</param>
        private SourceCode(string content, StorageFile? file)
        {
            _Content = content;
            File = file;
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
        /// Creates a new <see cref="SourceCode"/> instance with no linked file
        /// </summary>
        /// <returns>A new, empty <see cref="SourceCode"/> instance</returns>
        [Pure]
        public static SourceCode CreateEmpty() => new SourceCode("\r", null);

        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance from the specified reference file
        /// </summary>
        /// <param name="file">The file to read the contents of the source code from</param>
        /// <returns>A new <see cref="SourceCode"/> instance with the contents of <paramref name="file"/></returns>
        [Pure]
        public static async Task<SourceCode> LoadFromReferenceFileAsync(StorageFile file)
        {
            string text = await FileIO.ReadTextAsync(file);

            return new SourceCode(text, null);
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

                return new SourceCode(text, file);
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
            Guard.MustBeNotNull(File, nameof(File));

            try
            {
                await FileIO.WriteTextAsync(File, Content);

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
        public async Task<bool> TrySaveAsAsync(StorageFile file)
        {
            try
            {
                await FileIO.WriteTextAsync(file, Content);

                File = file;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
