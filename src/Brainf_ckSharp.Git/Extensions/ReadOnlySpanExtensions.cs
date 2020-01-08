using System.Diagnostics.Contracts;
using System.Numerics;
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
        /// Counts the number of occurrences of a given character into a target <see cref="ReadOnlySpan{T}"/> instance
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to read</param>
        /// <param name="c">The character to look for</param>
        /// <returns>The number of occurrences of <paramref name="c"/> in <paramref name="span"/></returns>
        [Pure]
        public static int Count(this ReadOnlySpan<char> span, char c)
        {
            // Get a reference to the first string character
            ref char r0 = ref MemoryMarshal.GetReference(span);
            int length = span.Length;
            int i = 0, result = 0;

            /* Only execute the SIMD-enabled branch if the Vector<T> APIs
             * are hardware accelerated on the current CPU, and if the
             * source span has at least Vector<ushort>.Count items to check.
             * Vector<char> is not supported, but the type is equivalent to
             * ushort anyway, as they're both unsigned 16 bits integers. */
            if (Vector.IsHardwareAccelerated &&
                i + Vector<ushort>.Count < length)
            {
                int end = length - Vector<ushort>.Count;
                Vector<ushort> partials = Vector<ushort>.Zero;

                for (; i < end; i += Vector<ushort>.Count)
                {
                    ref char ri = ref Unsafe.Add(ref r0, i);

                    /* Load the current Vector<ushort> register.
                     * Vector.Equals sets matching positions to all 1s, and
                     * Vector.BitwiseAnd results in a Vector<ushort> with 1
                     * in positions corresponding to matching characters,
                     * and 0 otherwise. The final += is also calling the
                     * right vectorized instruction automatically. */
                    Vector<ushort> vi = Unsafe.As<char, Vector<ushort>>(ref ri);
                    Vector<ushort> vc = new Vector<ushort>(c);
                    Vector<ushort> ve = Vector.Equals(vi, vc);
                    Vector<ushort> va = Vector.BitwiseAnd(ve, Vector<ushort>.One);

                    partials += va;
                }

                // Compute the horizontal sum of the partial results
                result = Vector.Dot(partials, Vector<ushort>.One);
            }

            // Iterate over the remaining characters and count those that match
            for (; i < length; i++)
                if (Unsafe.Add(ref r0, i) == c)
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
