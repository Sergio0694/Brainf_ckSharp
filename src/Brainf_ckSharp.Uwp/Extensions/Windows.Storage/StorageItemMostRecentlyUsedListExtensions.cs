using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace Windows.Storage.AccessCache
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="StorageItemMostRecentlyUsedList"/> type
    /// </summary>
    public static class StorageItemMostRecentlyUsedListExtensions
    {
        /// <summary>
        /// Retrieves the specified file from the most recently used (MRU) list
        /// </summary>
        /// <param name="list">The source <see cref="StorageItemMostRecentlyUsedList"/> instance</param>
        /// <param name="token">The token of the file to retrieve</param>
        /// <returns>The target <see cref="StorageFile"/> instance, or <see langword="null"/> if the file was not found</returns>
        [Pure]
        public static async Task<StorageFile?> TryGetFileAsync(this StorageItemMostRecentlyUsedList list, string token)
        {
            try
            {
                return await list.GetFileAsync(token);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
