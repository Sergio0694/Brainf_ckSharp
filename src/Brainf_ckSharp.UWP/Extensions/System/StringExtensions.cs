using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
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

            int bufferLength = textLength * 2;
            using StackOnlyUnmanagedMemoryOwner<char> buffer = StackOnlyUnmanagedMemoryOwner<char>.Allocate(bufferLength);

            fixed (char* p = &buffer.GetReference())
            {
                ref char textRef = ref MemoryMarshal.GetReference(text);
                ref char bufferRef = ref Unsafe.AsRef<char>(p);

                // Alternate source characters with the separator
                for (int i = 0; i < textLength; i++)
                {
                    Unsafe.Add(ref bufferRef, i * 2) = Unsafe.Add(ref textRef, i);
                    Unsafe.Add(ref bufferRef, i * 2 + 1) = c;
                }

                return new string(p, 0, bufferLength);
            }
        }
    }
}
