using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Brainf_ckSharp.Uwp.Extensions.System
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="string"/> type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Creates a <see cref="string"/> by alternating a given character between ones in an input instance
        /// </summary>
        /// <param name="text">The input <see cref="ReadOnlySpan{T}"/> value, mapping the input text</param>
        /// <param name="c">The separator character to interleave</param>
        /// <returns>A new <see cref="string"/> instance with alternating source and separator characters</returns>
        [Pure]
        public static string InterleaveWithCharacter(this ReadOnlySpan<char> text, char c)
        {
            // Fallback for empty strings
            int sourceLength = text.Length;
            if (sourceLength == 0) return string.Empty;

            // Rent the temporary buffer
            int bufferLength = sourceLength * 2;
            char[] buffer = ArrayPool<char>.Shared.Rent(bufferLength);

            try
            {
                ref char textRef = ref MemoryMarshal.GetReference(text);
                ref char bufferRef = ref buffer[0];

                // Alternate source characters with the separator
                for (int i = 0; i < sourceLength; i++)
                {
                    Unsafe.Add(ref bufferRef, i * 2) = Unsafe.Add(ref textRef, i);
                    Unsafe.Add(ref bufferRef, i * 2 + 1) = c;
                }

                // Create the new string with the correct view on the temporary buffer
                return new string(buffer, 0, bufferLength);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }
    }
}
