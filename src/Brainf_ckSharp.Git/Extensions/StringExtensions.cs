using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="string"/> type
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Counts the number of occurrences of a given character into a target <see cref="string"/>
        /// </summary>
        /// <param name="text">The input text to read</param>
        /// <param name="c">The character to look for</param>
        /// <returns>The number of occurrences of <paramref name="c"/> in <paramref name="text"/></returns>
        [Pure]
        public static int Count(this string text, char c)
        {
            int length = text.Length;

            // Empty string, just return 0
            if (length == 0) return 0;

            ref char r0 = ref MemoryMarshal.GetReference(text.AsSpan());
            int result = 0;

            // Go over the input text and look for the character
            for (int i = 0; i < length; i++)
                if (Unsafe.Add(ref r0, i) == c)
                    result++;

            return result;
        }
    }
}
