using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.System;

namespace Brainf_ckSharp.Uwp.Controls.Ide.Helpers
{
    /// <summary>
    /// A <see langword="class"/> that generates text
    /// </summary>
    internal static class TextGenerator
    {
        /// <summary>
        /// Initializes the static data for <see cref="TextGenerator"/>
        /// </summary>
        static TextGenerator()
        {
            _CachedStrings = ArrayPool<string>.Shared.Rent(2);
            _CachedStrings[0] = "1\n";
            _CacheLength = 1;
        }

        /// <summary>
        /// The borrowed <see cref="string"/> array with the cached results
        /// </summary>
        private static string[] _CachedStrings;

        /// <summary>
        /// The current number of cached results into <see cref="_CachedStrings"/>
        /// </summary>
        private static int _CacheLength;

        /// <summary>
        /// Gets a <see cref="string"/> with the numbers in [1,n], one per line
        /// </summary>
        /// <param name="n">The target number of lines</param>
        /// <returns>A <see cref="string"/> with the numbers in [1,n], one per line</returns>
        [Pure]
        public static unsafe string GetLineNumbersText(int n)
        {
            Debug.Assert(n >= 1);

            // Rent a new array and copy the previous results, if the cache is too small
            if (n >= _CachedStrings.Length)
            {
                string[] updatedCache = ArrayPool<string>.Shared.Rent(n);

                _CachedStrings.AsSpan(0, _CacheLength).CopyTo(updatedCache);

                ArrayPool<string>.Shared.Return(_CachedStrings);

                _CachedStrings = updatedCache;
            }

            // Compute and cache results if needed
            if (n >= _CacheLength)
            {
                // The temporary buffer is allocated outside of the loop
                // because otherwise it'd cause a temporary memory leak.
                // Using a stack buffer avoids an extra allocation.
                char* p0 = stackalloc char[10];

                ref string r0 = ref _CachedStrings[0];

                for (int i = _CacheLength; i < n; i++)
                {
                    // Prepare the buffer for the current value
                    uint target = (uint)i + 1;
                    int numberOfDigits = target.CountDigits();
                    target.ToString(p0, numberOfDigits);
                    p0[numberOfDigits] = '\n';

                    // Build the new concatenated string
                    string left = Unsafe.Add(ref r0, i - 1);
                    string right = new(p0, 0, numberOfDigits + 1);
                    string combined = string.Concat(left, right);

                    // Store it in the cache
                    Unsafe.Add(ref r0, i) = combined;
                }

                _CacheLength = n;
            }

            // The cache is 0-based and starts with "1\n"
            return _CachedStrings[n - 1];
        }
    }
}
