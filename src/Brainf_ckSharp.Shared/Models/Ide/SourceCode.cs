using System;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Threading.Tasks;
using Brainf_ckSharp.Services;
using Microsoft.Toolkit.Diagnostics;

#nullable enable

namespace Brainf_ckSharp.Shared.Models.Ide
{
    /// <summary>
    /// A model that represents a source code
    /// </summary>
    public sealed class SourceCode
    {
        /// <summary>
        /// Gets the empty content for a new source code
        /// </summary>
        public const string EmptyContent = "\r";

        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance with the specified parameters
        /// </summary>
        /// <param name="content">The content of the current source code</param>
        /// <param name="file">The <see cref="IFile"/> instance for the current source code</param>
        /// <param name="metadata">The metadata for the current file</param>
        private SourceCode(string content, IFile? file, CodeMetadata metadata)
        {
            Content = content;
            File = file;
            Metadata = metadata;
        }

        /// <summary>
        /// Gets or sets the content of the current source code
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFile"/> instance for the current source code
        /// </summary>
        public IFile? File { get; private set; }

        /// <summary>
        /// Gets the associated <see cref="CodeMetadata"/> instance for the current entry
        /// </summary>
        public CodeMetadata Metadata { get; }

        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance with no linked file
        /// </summary>
        /// <returns>A new, empty <see cref="SourceCode"/> instance</returns>
        [Pure]
        public static SourceCode CreateEmpty() => new SourceCode(EmptyContent, null, new CodeMetadata());

        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance from the specified reference file
        /// </summary>
        /// <param name="file">The file to read the contents of the source code from</param>
        /// <returns>A new <see cref="SourceCode"/> instance with the contents of <paramref name="file"/></returns>
        [Pure]
        public static async Task<SourceCode> LoadFromReferenceFileAsync(IFile file)
        {
            string text = await file.ReadAllTextAsync();

            return new SourceCode(text, null, CodeMetadata.Default);
        }

        /// <summary>
        /// Creates a new <see cref="SourceCode"/> instance from the specified editable file
        /// </summary>
        /// <param name="file">The file to read the contents of the source code from</param>
        /// <returns>A new <see cref="IFile"/> instance with the contents of <paramref name="file"/></returns>
        [Pure]
        public static async Task<SourceCode?> TryLoadFromEditableFileAsync(IFile file)
        {
            try
            {
                string text = await file.ReadAllTextAsync();

                text = text.Replace(Environment.NewLine, "\r");

                SourceCode code = new SourceCode(text, file, new CodeMetadata());

                string metadata = JsonSerializer.Serialize(code.Metadata);

                file.RequestFutureAccessPermission(metadata);

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
                await File!.WriteAllTextAsync(Content);

                string metadata = JsonSerializer.Serialize(Metadata);

                File!.RequestFutureAccessPermission(metadata);

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
        public Task<bool> TrySaveAsAsync(IFile file)
        {
            File = file;

            return TrySaveAsync();
        }
    }
}
