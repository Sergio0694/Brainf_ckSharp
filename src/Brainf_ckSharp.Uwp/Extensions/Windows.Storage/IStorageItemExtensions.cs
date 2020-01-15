using System;
using System.Diagnostics.Contracts;
using System.IO;
using Windows.ApplicationModel;

namespace Windows.Storage
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="IStorageItem"/> type
    /// </summary>
    public static class IStorageItemExtensions
    {
        /// <summary>
        /// Gets a unique id for a given stored file
        /// </summary>
        /// <param name="item">The input <see cref="IStorageItem"/> instance to identify</param>
        /// <returns>A <see cref="string"/> representing a unique id to identify the file on the drive</returns>
        [Pure]
        public static string GetId(this IStorageItem item)
        {
            return item.Path.GetxxHash32Code().ToHex();
        }

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

        /// <summary>
        /// Gets a formatted path <see cref="string"/> with a " » " separator
        /// </summary>
        /// <param name="item">The input <see cref="IStorageItem"/> instance to analyze</param>
        /// <returns>A formatted path <see cref="string"/> for <paramref name="item"/></returns>
        [Pure]
        public static unsafe string GetFormattedPath(this IStorageItem item)
        {
            const int separatorLength = 3; // " » "
            int
                numberOfSeparators = item.Path.Count(Path.DirectorySeparatorChar),
                formattedLength = item.Path.Length + numberOfSeparators * (separatorLength - 1) + separatorLength;

            /* The temporary buffer has space for one extra separator that is
             * always initialized even if it's not used in the final string.
             * This is done to avoid having to check the current index in the
             * main loop, which would be needed to check whether or not
             * the current separator should be written or not to the buffer. */
            char* p = stackalloc char[formattedLength];

            // Write the path parts
            int i = 0;
            foreach (ReadOnlySpan<char> part in item.Path.Tokenize(Path.DirectorySeparatorChar))
            {
                part.CopyTo(new Span<char>(p + i, part.Length));

                i += part.Length;

                // Write the characters manually to avoid another stackalloc
                p[i] = ' ';
                p[i + 1] = '»';
                p[i + 2] = ' ';

                i += 3;
            }

            // Create a string from the buffer and skip the last separator
            return new string(p, 0, formattedLength - 3);
        }
    }
}
