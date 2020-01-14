using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="string"/> type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Counts the number of occurrences of a given character into a target <see cref="string"/>
        /// </summary>
        /// <param name="text">The input text to read</param>
        /// <param name="c">The character to look for</param>
        /// <returns>The number of occurrences of <paramref name="c"/> in <paramref name="text"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(this string text, char c) => text.AsSpan().Count(c);

        /// <summary>
        /// Creates a new <see cref="ReadOnlySpanTokenizer{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="text">The target text to tokenize</param>
        /// <param name="separator">The separator character to use</param>
        /// <returns>A <see cref="ReadOnlySpanTokenizer{T}"/> instance working on <paramref name="text"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpanTokenizer<char> Tokenize(this string text, char separator) => new ReadOnlySpanTokenizer<char>(text.AsSpan(), separator);

        /// <summary>
        /// Gets a content hash from the input <see cref="string"/> instance using the xxHash32 algorithm
        /// </summary>
        /// <param name="text">The input <see cref="string"/> instance</param>
        /// <returns>The xxHash32 value for the input <see cref="string"/> instance</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetxxHash32Code(this string text) => text.AsSpan().GetxxHash32Code();
    }
}
