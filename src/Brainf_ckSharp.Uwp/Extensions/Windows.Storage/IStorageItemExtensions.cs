using Windows.ApplicationModel;
using Windows.Storage;
using JetBrains.Annotations;

namespace Brainf_ckSharp.Uwp.Extensions.Windows.Storage
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="IStorageItem"/> type
    /// </summary>
    public static class IStorageItemExtensions
    {
        /// <summary>
        /// Checks whether or not a given <see cref="IStorageItem"/> instance belongs to the installation directory
        /// </summary>
        /// <param name="item">The input <see cref="IStorageItem"/> instance to check</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is in the installation directory, <see langword="false"/> otherwise</returns>
        [Pure]
        public static bool IsFromPackageDirectory(this IStorageItem item)
        {
            return item.Path.StartsWith(Package.Current.InstalledLocation.Path);
        }
    }
}
