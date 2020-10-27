using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Brainf_ckSharp.Services
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="IFile"/> type
    /// </summary>
    public static class IFileExtensions
    {
        /// <summary>
        /// Reads all the text from a given file
        /// </summary>
        /// <param name="file">The input <see cref="IFile"/> instance to read from</param>
        /// <returns>The text read from <paramref name="file"/></returns>
        public static async Task<string> ReadAllTextAsync(this IFile file)
        {
            using Stream stream = await file.OpenStreamForReadAsync();
            using StreamReader reader = new(stream);

            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Writes all the text to a given file
        /// </summary>
        /// <param name="file">The target <see cref="IFile"/> instance to write to</param>
        /// <param name="text">The text to write to <paramref name="file"/></param>
        /// <returns>A <see cref="Task"/> that completes when the text has been written</returns>
        public static async Task WriteAllTextAsync(this IFile file, string text)
        {
            using Stream stream = await file.OpenStreamForWriteAsync();

            // Clear the current content
            if (stream.Length > 0) stream.SetLength(0);

            using StreamWriter writer = new(stream, Encoding.UTF8);

            await writer.WriteAsync(text);
        }
    }
}
