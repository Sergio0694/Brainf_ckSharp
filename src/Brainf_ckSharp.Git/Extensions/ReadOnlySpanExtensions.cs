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
            int i = 0, result;

            /* Only execute the SIMD-enabled branch if the Vector<T> APIs
             * are hardware accelerated on the current CPU.
             * Vector<char> is not supported, but the type is equivalent to
             * ushort anyway, as they're both unsigned 16 bits integers. */
            if (Vector.IsHardwareAccelerated)
            {
                int end = length - Vector<ushort>.Count;

                Vector<short> partials = Vector<short>.Zero;
                Vector<ushort> vc = new Vector<ushort>(c);

                for (; i <= end; i += Vector<ushort>.Count)
                {
                    ref char ri = ref Unsafe.Add(ref r0, i);

                    /* Load the current Vector<ushort> register.
                     * Vector.Equals sets matching positions to all 1s, and
                     * Vector.BitwiseAnd results in a Vector<ushort> with 1
                     * in positions corresponding to matching characters,
                     * and 0 otherwise. The final += is also calling the
                     * right vectorized instruction automatically. */
                    Vector<ushort> vi = Unsafe.As<char, Vector<ushort>>(ref ri);
                    Vector<ushort> ve = Vector.Equals(vi, vc);
                    Vector<short> vs = Vector.AsVectorInt16(ve);

                    partials -= vs;
                }

                // Compute the horizontal sum of the partial results
                result = Vector.Dot(partials, Vector<short>.One);
            }
            else result = 0;

            // Iterate over the remaining characters and count those that match
            for (; i < length; i++)
            {
                /* Skip a conditional jump by assigning the comparison
                 * result to a variable and reinterpreting a reference to
                 * it as a byte reference. The byte value is then implicitly
                 * cast to int before adding it to the result. */
                bool equals = Unsafe.Add(ref r0, i) == c;
                result += Unsafe.As<bool, byte>(ref equals);
            }

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
        public static int GetxxHash32Code<T>(this ReadOnlySpan<T> span) where T : unmanaged
        {
            // Get a reference to the input span and reinterpret as a byte sequence
            ref T r0 = ref MemoryMarshal.GetReference(span);
            ref byte r1 = ref Unsafe.As<T, byte>(ref r0);
            int
                queue = 0,
                length = span.Length * Unsafe.SizeOf<T>(),
                i = 0;

            // Dedicated SIMD branch, if available
            if (Vector.IsHardwareAccelerated)
            {
                /* Test whether the total number of bytes is at least
                 * equal to the number that can fit in a single SIMD register.
                 * If that is not the case, skip the entire SIMD branch, which
                 * also saves the unnecessary computation of partial hash
                 * values from the accumulation register, and the loading
                 * of the prime constant in the secondary SIMD register. */
                if (length >= Vector<int>.Count * sizeof(int))
                {
                    Vector<int> vh = Vector<int>.Zero;
                    Vector<int> v397 = new Vector<int>(397);

                    /* First upper bound for the vectorized path.
                     * The first loop has sequences of 8 SIMD operations unrolled,
                     * so assuming that a SIMD register can hold 8 int values at a time,
                     * it processes 8 * Vector<int>.Count * sizeof(int), which results in
                     * 128 bytes on SSE registers, 256 on AVX2 and 512 on AVX512 registers. */
                    int end256 = length - 8 * Vector<int>.Count * sizeof(int);

                    for (; i <= end256; i += 8 * Vector<int>.Count * sizeof(int))
                    {
                        ref byte ri0 = ref Unsafe.Add(ref r1, Vector<int>.Count * sizeof(int) * 0 + i);
                        Vector<int> vi0 = Unsafe.ReadUnaligned<Vector<int>>(ref ri0);
                        Vector<int> vp0 = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp0, vi0);

                        ref byte ri1 = ref Unsafe.Add(ref r1, Vector<int>.Count * sizeof(int) * 1 + i);
                        Vector<int> vi1 = Unsafe.ReadUnaligned<Vector<int>>(ref ri1);
                        Vector<int> vp1 = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp1, vi1);

                        ref byte ri2 = ref Unsafe.Add(ref r1, Vector<int>.Count * sizeof(int) * 2 + i);
                        Vector<int> vi2 = Unsafe.ReadUnaligned<Vector<int>>(ref ri2);
                        Vector<int> vp2 = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp2, vi2);

                        ref byte ri3 = ref Unsafe.Add(ref r1, Vector<int>.Count * sizeof(int) * 3 + i);
                        Vector<int> vi3 = Unsafe.ReadUnaligned<Vector<int>>(ref ri3);
                        Vector<int> vp3 = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp3, vi3);

                        ref byte ri4 = ref Unsafe.Add(ref r1, Vector<int>.Count * sizeof(int) * 4 + i);
                        Vector<int> vi4 = Unsafe.ReadUnaligned<Vector<int>>(ref ri4);
                        Vector<int> vp4 = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp4, vi4);

                        ref byte ri5 = ref Unsafe.Add(ref r1, Vector<int>.Count * sizeof(int) * 5 + i);
                        Vector<int> vi5 = Unsafe.ReadUnaligned<Vector<int>>(ref ri5);
                        Vector<int> vp5 = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp5, vi5);

                        ref byte ri6 = ref Unsafe.Add(ref r1, Vector<int>.Count * sizeof(int) * 6 + i);
                        Vector<int> vi6 = Unsafe.ReadUnaligned<Vector<int>>(ref ri6);
                        Vector<int> vp6 = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp6, vi6);

                        ref byte ri7 = ref Unsafe.Add(ref r1, Vector<int>.Count * sizeof(int) * 7 + i);
                        Vector<int> vi7 = Unsafe.ReadUnaligned<Vector<int>>(ref ri7);
                        Vector<int> vp7 = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp7, vi7);
                    }

                    /* Second upper bound for the vectorized path.
                     * Each iteration processes 16 bytes on SSE, 32 bytes on AVX2
                     * and 64 on AVX512 registers. When this point is reached,
                     * it means that there are at most 127 bytes remaining on SSE,
                     * or 255 on AVX2, or 511 on AVX512 systems.*/
                    int end32 = length - i - Vector<int>.Count * sizeof(int);

                    for (; i <= end32; i += Vector<int>.Count * sizeof(int))
                    {
                        ref byte ri = ref Unsafe.Add(ref r1, i);
                        Vector<int> vi = Unsafe.ReadUnaligned<Vector<int>>(ref ri);
                        Vector<int> vp = Vector.Multiply(vh, v397);
                        vh = Vector.Xor(vp, vi);
                    }

                    /* Combine the partial hash values in each position.
                     * The Vector<int>.Count is a compile time constant
                     * for each generic type for the JIT, so the branches
                     * not taken are completely eliminated in asm.
                     * The loop below is actually already unrolled by the JIT, but
                     * doing so manually results in slightly more efficient asm.*/
                    if (Vector<int>.Count == 4)
                    {
                        // SSE
                        queue = unchecked((queue * 397) ^ vh[0]);
                        queue = unchecked((queue * 397) ^ vh[1]);
                        queue = unchecked((queue * 397) ^ vh[2]);
                        queue = unchecked((queue * 397) ^ vh[3]);
                    }
                    else if (Vector<int>.Count == 8)
                    {
                        // AVX2
                        queue = unchecked((queue * 397) ^ vh[0]);
                        queue = unchecked((queue * 397) ^ vh[1]);
                        queue = unchecked((queue * 397) ^ vh[2]);
                        queue = unchecked((queue * 397) ^ vh[3]);
                        queue = unchecked((queue * 397) ^ vh[4]);
                        queue = unchecked((queue * 397) ^ vh[5]);
                        queue = unchecked((queue * 397) ^ vh[6]);
                        queue = unchecked((queue * 397) ^ vh[7]);
                    }
                    else if (Vector<int>.Count == 16)
                    {
                        // AVX512
                        queue = unchecked((queue * 397) ^ vh[0]);
                        queue = unchecked((queue * 397) ^ vh[1]);
                        queue = unchecked((queue * 397) ^ vh[2]);
                        queue = unchecked((queue * 397) ^ vh[3]);
                        queue = unchecked((queue * 397) ^ vh[4]);
                        queue = unchecked((queue * 397) ^ vh[5]);
                        queue = unchecked((queue * 397) ^ vh[6]);
                        queue = unchecked((queue * 397) ^ vh[7]);
                        queue = unchecked((queue * 397) ^ vh[8]);
                        queue = unchecked((queue * 397) ^ vh[9]);
                        queue = unchecked((queue * 397) ^ vh[10]);
                        queue = unchecked((queue * 397) ^ vh[11]);
                        queue = unchecked((queue * 397) ^ vh[12]);
                        queue = unchecked((queue * 397) ^ vh[13]);
                        queue = unchecked((queue * 397) ^ vh[14]);
                        queue = unchecked((queue * 397) ^ vh[15]);
                    }
                    else
                    {
                        // Fallback on different register sizes
                        for (int j = 0; j < Vector<int>.Count; j++)
                        {
                            queue = unchecked((queue * 397) ^ vh[j]);
                        }
                    }
                }

                /* At this point, regardless of whether or not the previous
                 * branch was taken, there are at most 15 unprocessed bytes
                 * on SSE systems, 31 on AVX2 systems and 63 on AVX512 systems. */
            }
            else
            {
                // Process groups of 64 bytes at a time
                int end64 = length - 8 * sizeof(ulong);

                for (; i <= end64; i += 8 * sizeof(ulong))
                {
                    ref byte ri0 = ref Unsafe.Add(ref r1, sizeof(ulong) * 0 + i);
                    ulong value0 = Unsafe.ReadUnaligned<ulong>(ref ri0);
                    queue = unchecked((queue * 397) ^ (int)value0 ^ (int)(value0 >> 32));

                    ref byte ri1 = ref Unsafe.Add(ref r1, sizeof(ulong) * 1 + i);
                    ulong value1 = Unsafe.ReadUnaligned<ulong>(ref ri1);
                    queue = unchecked((queue * 397) ^ (int)value1 ^ (int)(value1 >> 32));

                    ref byte ri2 = ref Unsafe.Add(ref r1, sizeof(ulong) * 2 + i);
                    ulong value2 = Unsafe.ReadUnaligned<ulong>(ref ri2);
                    queue = unchecked((queue * 397) ^ (int)value2 ^ (int)(value2 >> 32));

                    ref byte ri3 = ref Unsafe.Add(ref r1, sizeof(ulong) * 3 + i);
                    ulong value3 = Unsafe.ReadUnaligned<ulong>(ref ri3);
                    queue = unchecked((queue * 397) ^ (int)value3 ^ (int)(value3 >> 32));

                    ref byte ri4 = ref Unsafe.Add(ref r1, sizeof(ulong) * 4 + i);
                    ulong value4 = Unsafe.ReadUnaligned<ulong>(ref ri4);
                    queue = unchecked((queue * 397) ^ (int)value4 ^ (int)(value4 >> 32));

                    ref byte ri5 = ref Unsafe.Add(ref r1, sizeof(ulong) * 5 + i);
                    ulong value5 = Unsafe.ReadUnaligned<ulong>(ref ri5);
                    queue = unchecked((queue * 397) ^ (int)value5 ^ (int)(value5 >> 32));

                    ref byte ri6 = ref Unsafe.Add(ref r1, sizeof(ulong) * 6 + i);
                    ulong value6 = Unsafe.ReadUnaligned<ulong>(ref ri6);
                    queue = unchecked((queue * 397) ^ (int)value6 ^ (int)(value6 >> 32));

                    ref byte ri7 = ref Unsafe.Add(ref r1, sizeof(ulong) * 7 + i);
                    ulong value7 = Unsafe.ReadUnaligned<ulong>(ref ri7);
                    queue = unchecked((queue * 397) ^ (int)value7 ^ (int)(value7 >> 32));
                }

                /* At this point, there are up to 63 bytes left.
                 * If there are at least 32, unroll that iteration with
                 * the same procedure as before, but as uint values. */
                if (length - i >= 8 * sizeof(uint))
                {
                    ref byte ri0 = ref Unsafe.Add(ref r1, sizeof(uint) * 0 + i);
                    uint value0 = Unsafe.ReadUnaligned<uint>(ref ri0);
                    queue = unchecked((queue * 397) ^ (int)value0);

                    ref byte ri1 = ref Unsafe.Add(ref r1, sizeof(uint) * 1 + i);
                    uint value1 = Unsafe.ReadUnaligned<uint>(ref ri1);
                    queue = unchecked((queue * 397) ^ (int)value1);

                    ref byte ri2 = ref Unsafe.Add(ref r1, sizeof(uint) * 2 + i);
                    uint value2 = Unsafe.ReadUnaligned<uint>(ref ri2);
                    queue = unchecked((queue * 397) ^ (int)value2);

                    ref byte ri3 = ref Unsafe.Add(ref r1, sizeof(uint) * 3 + i);
                    uint value3 = Unsafe.ReadUnaligned<uint>(ref ri3);
                    queue = unchecked((queue * 397) ^ (int)value3);

                    ref byte ri4 = ref Unsafe.Add(ref r1, sizeof(uint) * 4 + i);
                    uint value4 = Unsafe.ReadUnaligned<uint>(ref ri4);
                    queue = unchecked((queue * 397) ^ (int)value4);

                    ref byte ri5 = ref Unsafe.Add(ref r1, sizeof(uint) * 5 + i);
                    uint value5 = Unsafe.ReadUnaligned<uint>(ref ri5);
                    queue = unchecked((queue * 397) ^ (int)value5);

                    ref byte ri6 = ref Unsafe.Add(ref r1, sizeof(uint) * 6 + i);
                    uint value6 = Unsafe.ReadUnaligned<uint>(ref ri6);
                    queue = unchecked((queue * 397) ^ (int)value6);

                    ref byte ri7 = ref Unsafe.Add(ref r1, sizeof(uint) * 7 + i);
                    uint value7 = Unsafe.ReadUnaligned<uint>(ref ri7);
                    queue = unchecked((queue * 397) ^ (int)value7);
                }

                // The non-SIMD path leaves up to 31 unprocessed bytes
            }

            /* At this point there might be up to 31 bytes left on both AVX2 systems,
             * and on systems with no hardware accelerated SIMD registers.
             * That number would go up to 63 on AVX512 systems, in which case it is
             * still useful to perform this last loop unrolling.
             * The only case where this branch is never taken is on SSE systems,
             * but since those are not so common anyway the code is left here for simplicity.
             * What follows is the same procedure as before, but with ushort values,
             * so that if there are at least 16 bytes available, those
             * will all be processed in a single unrolled iteration. */
            if (length - i >= 8 * sizeof(ushort))
            {
                ref byte ri0 = ref Unsafe.Add(ref r1, sizeof(ushort) * 0 + i);
                ushort value0 = Unsafe.ReadUnaligned<ushort>(ref ri0);
                queue = unchecked((queue * 397) ^ value0);

                ref byte ri1 = ref Unsafe.Add(ref r1, sizeof(ushort) * 1 + i);
                ushort value1 = Unsafe.ReadUnaligned<ushort>(ref ri1);
                queue = unchecked((queue * 397) ^ value1);

                ref byte ri2 = ref Unsafe.Add(ref r1, sizeof(ushort) * 2 + i);
                ushort value2 = Unsafe.ReadUnaligned<ushort>(ref ri2);
                queue = unchecked((queue * 397) ^ value2);

                ref byte ri3 = ref Unsafe.Add(ref r1, sizeof(ushort) * 3 + i);
                ushort value3 = Unsafe.ReadUnaligned<ushort>(ref ri3);
                queue = unchecked((queue * 397) ^ value3);

                ref byte ri4 = ref Unsafe.Add(ref r1, sizeof(ushort) * 4 + i);
                ushort value4 = Unsafe.ReadUnaligned<ushort>(ref ri4);
                queue = unchecked((queue * 397) ^ value4);

                ref byte ri5 = ref Unsafe.Add(ref r1, sizeof(ushort) * 5 + i);
                ushort value5 = Unsafe.ReadUnaligned<ushort>(ref ri5);
                queue = unchecked((queue * 397) ^ value5);

                ref byte ri6 = ref Unsafe.Add(ref r1, sizeof(ushort) * 6 + i);
                ushort value6 = Unsafe.ReadUnaligned<ushort>(ref ri6);
                queue = unchecked((queue * 397) ^ value6);

                ref byte ri7 = ref Unsafe.Add(ref r1, sizeof(ushort) * 7 + i);
                ushort value7 = Unsafe.ReadUnaligned<ushort>(ref ri7);
                queue = unchecked((queue * 397) ^ value7);
            }

            // Handle the leftover items
            for (; i < length; i++)
            {
                queue = unchecked((queue * 397) ^ Unsafe.Add(ref r1, i));
            }

            const uint Prime1 = 2654435761U;
            const uint Prime2 = 2246822519U;
            const uint Prime3 = 3266489917U;
            const uint Prime4 = 668265263U;

            /* The following code replicates (with some simplification) the
             * xxHash32 procedure from the System.HashCode type. The reason
             * this is reimplemented here is so that the seed is static and
             * not randomly initialized at every startup.
             * This guarantees repeatable hash codes if the data is the same. */
            uint hash = Prime1 + (uint)queue + Prime3;

            hash = (hash << 17) | (hash >> (32 - 17));
            hash *= Prime4;
            hash ^= hash >> 15;
            hash *= Prime2;
            hash ^= hash >> 13;
            hash *= Prime3;
            hash ^= hash >> 16;

            return (int)hash;
        }
    }
}
