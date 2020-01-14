using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="ReadOnlySpan{T}"/> type
    /// </summary>
    public static class ReadOnlySpanExtensions
    {
        /// <summary>
        /// Returns a <see cref="ReadOnlySpanEnumerator{T}"/> instance that enumerates the items in the input <see cref="ReadOnlySpan{T}"/> instance
        /// </summary>
        /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> to enumerate</param>
        /// <returns>A<see cref="ReadOnlySpanEnumerator{T}"/> instance that enumerates the items in <paramref name="span"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpanEnumerator<T> Enumerate<T>(this ReadOnlySpan<T> span) => new ReadOnlySpanEnumerator<T>(span);
    }
}
