using System;
using System.Diagnostics.Contracts;
using System.IO;
using Brainf_ckSharp.Services;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace Windows.Storage
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="IFile"/> type
    /// </summary>
    public static class IFileExtensions
    {
        /// <summary>
        /// Gets a formatted path <see cref="string"/> with a " » " separator
        /// </summary>
        /// <param name="file">The input <see cref="IFile"/> instance to analyze</param>
        /// <returns>A formatted path <see cref="string"/> for <paramref name="file"/></returns>
        [Pure]
        public static unsafe string GetFormattedPath(this IFile file)
        {
            const int separatorLength = 3; // " » "
            int
                numberOfSeparators = file.Path.Count(Path.DirectorySeparatorChar),
                formattedLength = file.Path.Length + numberOfSeparators * (separatorLength - 1) + separatorLength;

            // The temporary buffer has space for one extra separator that is
            // always initialized even if it's not used in the final string.
            // This is done to avoid having to check the current index in the
            // main loop, which would be needed to check whether or not
            // the current separator should be written or not to the buffer.
            using SpanOwner<char> buffer = SpanOwner<char>.Allocate(formattedLength);

            fixed (char* p = &buffer.DangerousGetReference())
            {
                // Write the path parts
                int i = 0;
                foreach (ReadOnlySpan<char> part in file.Path.Tokenize(Path.DirectorySeparatorChar))
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
                return new(p, 0, formattedLength - 3);
            }
        }
    }
}
