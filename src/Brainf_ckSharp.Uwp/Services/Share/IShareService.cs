using Windows.Storage;

namespace Brainf_ckSharp.Uwp.Services.Share
{
    /// <summary>
    /// The default <see langword="interface"/> for the a service that can share data from the current application
    /// </summary>
    public interface IShareService
    {
        /// <summary>
        /// Shares a file from the current applicaation
        /// </summary>
        /// <param name="title">The title of the file to share</param>
        /// <param name="file">The file to share</param>
        void Share(string title, StorageFile file);
    }
}
