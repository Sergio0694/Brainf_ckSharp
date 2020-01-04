using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="ReadOnlySpan{T}"/> type
    /// </summary>
    public static class ReadOnlySpanExtensions
    {
        /// <summary>
        /// Counts the number of occurrences of a given <typeparamref name="T"/> item into a target <see cref="ReadOnlySpan{T}"/> instance
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to read</param>
        /// <param name="c">The <typeparamref name="T"/> item to look for</param>
        /// <returns>The number of occurrences of <paramref name="c"/> in <paramref name="span"/></returns>
        [Pure]
        public static int Count<T>(this ReadOnlySpan<T> span, T c) where T : IEquatable<T>
        {
            int length = span.Length;

            // Empty span, just return 0
            if (length == 0) return 0;

            ref T r0 = ref MemoryMarshal.GetReference(span);
            int result = 0;

            // Go over the input text and look for the character
            for (int i = 0; i < length; i++)
                if (Unsafe.Add(ref r0, i).Equals(c))
                    result++;

            return result;
        }

        /// <summary>
        /// Creates a new <see cref="ReadOnlySpanTokenizer{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="span">The target <see cref="ReadOnlySpan{T}"/> to tokenize</param>
        /// <param name="separator">The separator <typeparamref name="T"/> item to use</param>
        /// <returns>A <see cref="ReadOnlySpanTokenizer{T}"/> instance working on <paramref name="span"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpanTokenizer<T> Tokenize<T>(this ReadOnlySpan<T> span, T separator) where T : IEquatable<T>
        {
            return new ReadOnlySpanTokenizer<T>(span, separator);
        }

        /// <summary>
        /// Gets a content hash from the input <see cref="ReadOnlySpan{T}"/> instance using the xxHash32 algorithm
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="ReadOnlySpan{T}"/> instance</typeparam>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance</param>
        /// <returns>The xxHash32 value for the input <see cref="ReadOnlySpan{T}"/> instance</returns>
        [Pure]
        public static int GetxxHash32Code<T>(this ReadOnlySpan<T> span)
        {
            int length = span.Length;

            // Fallback if the span is empty
            if (length == 0) return 0;

            ref T r0 = ref MemoryMarshal.GetReference(span);
            int
                hash = 0,
                end = length - 8,
                i = 0;

            // Vectorize with as many items at once as possible
            for (; i < end; i += 7)
            {
                hash = HashCode.Combine(
                    hash,
                    Unsafe.Add(ref r0, i),
                    Unsafe.Add(ref r0, i + 1),
                    Unsafe.Add(ref r0, i + 2),
                    Unsafe.Add(ref r0, i + 3),
                    Unsafe.Add(ref r0, i + 4),
                    Unsafe.Add(ref r0, i + 5),
                    Unsafe.Add(ref r0, i + 6));
            }

            // Handle the leftover items
            for (; i < length; i++)
            {
                hash = HashCode.Combine(hash, Unsafe.Add(ref r0, i));
            }

            return hash;
        }
    }
}
