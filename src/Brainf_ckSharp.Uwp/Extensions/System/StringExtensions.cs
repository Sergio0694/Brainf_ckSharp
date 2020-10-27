using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace Brainf_ckSharp.Uwp.Extensions.System
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="string"/> type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Creates a <see cref="string"/> by alternating a given character between ones in an input <see cref="ReadOnlySpan{T}"/>
        /// </summary>
        /// <param name="text">The input <see cref="ReadOnlySpan{T}"/> value, mapping the input text</param>
        /// <param name="c">The separator character to interleave</param>
        /// <returns>A new <see cref="string"/> instance with alternating source and separator characters</returns>
        [Pure]
        public static unsafe string InterleaveWithCharacter(this ReadOnlySpan<char> text, char c)
        {
            int textLength = text.Length;

            if (textLength == 0) return string.Empty;

            using SpanOwner<char> buffer = SpanOwner<char>.Allocate(textLength * 2);

            ref char textRef = ref text.DangerousGetReference();
            ref char bufferRef = ref buffer.DangerousGetReference();

            // Alternate source characters with the separator
            for (int i = 0; i < textLength; i++)
            {
                Unsafe.Add(ref bufferRef, i * 2) = Unsafe.Add(ref textRef, i);
                Unsafe.Add(ref bufferRef, i * 2 + 1) = c;
            }

            // Create a string from the temporary buffer
            fixed (char* p = &bufferRef)
            {
                return new(p, 0, buffer.Length);
            }
        }
    }
}
