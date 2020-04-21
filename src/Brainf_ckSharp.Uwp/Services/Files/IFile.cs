using System;
using System.IO;
using System.Threading.Tasks;

namespace Brainf_ckSharp.Uwp.Services.Files
{
    /// <summary>
    /// An <see langword="interface"/> representing a file.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// Gets the path of the current file
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets whether or not the file can only be read from
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Loads the basic properties for the current file.
        /// </summary>
        /// <returns>The file size and the last edit time.</returns>
        Task<(ulong Size, DateTimeOffset EditTime)> GetPropertiesAsync();

        /// <summary>
        /// Opens a <see cref="Stream"/> for reading from the file
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> returning the requested <see cref="Stream"/></returns>
        Task<Stream> OpenStreamForReadAsync();

        /// <summary>
        /// Opens a <see cref="Stream"/> for writing to the file
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> returning the requested <see cref="Stream"/></returns>
        Task<Stream> OpenStreamForWriteAsync();

        /// <summary>
        /// Requests permission for access this file in the future without asking the user
        /// </summary>
        /// <param name="metadata">The optional metadata to associate with the file</param>
        void RequestFutureAccessPermission(string metadata);
    }
}
