using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="PinnedUnmanagedMemoryOwnerExtensions"/> type
    /// </summary>
    public static class PinnedUnmanagedMemoryOwnerExtensions
    {
        /// <summary>
        /// Creates a new <see cref="string"/> with the characters in the input <see cref="PinnedUnmanagedMemoryOwner{T}"/> instance
        /// </summary>
        /// <param name="memoryOwner">The <see cref="PinnedUnmanagedMemoryOwner{T}"/> instance to read characters from</param>
        /// <returns>A <see cref="string"/> with the characters in <paramref name="memoryOwner"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string AsString(this PinnedUnmanagedMemoryOwner<char> memoryOwner)
        {
            if (memoryOwner.Size == 0) return string.Empty;

            return new string(memoryOwner.GetPointer(), 0, memoryOwner.Size);
        }
    }
}
