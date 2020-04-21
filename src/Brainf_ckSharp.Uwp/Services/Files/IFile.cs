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
        /// Opens a <see cref="Stream"/> for reading from the file
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> returning the requested <see cref="Stream"/></returns>
        Task<Stream> OpenStreamForReadAsync();

        /// <summary>
        /// Opens a <see cref="Stream"/> for writing to the file
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> returning the requested <see cref="Stream"/></returns>
        Task<Stream> OpenStreamForWriteAsync();
    }
}
