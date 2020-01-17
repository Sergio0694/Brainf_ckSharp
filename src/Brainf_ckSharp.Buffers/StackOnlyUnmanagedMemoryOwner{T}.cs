using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers
{
    /// <summary>
    /// A <see langword="struct"/> that owns a memory buffer shared from a common pool
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    public ref struct StackOnlyUnmanagedMemoryOwner<T> where T : unmanaged
    {
        /// <summary>
        /// The size of the usable buffer within <see cref="Buffer"/>
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// The <see cref="byte"/> array that constitutes the memory buffer for the current instance
        /// </summary>
        private readonly byte[] Buffer;

        /// <summary>
        /// Creates a new <see cref="StackOnlyUnmanagedMemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        private unsafe StackOnlyUnmanagedMemoryOwner(int size, bool clear)
        {
            DebugGuard.MustBeGreaterThan(size, 0, nameof(size));

            Size = size;
            Buffer = ArrayPool<byte>.Shared.Rent(size * sizeof(T));

            if (clear) Buffer.AsSpan(0, size).Clear();
        }

        /// <summary>
        /// Creates a new <see cref="StackOnlyUnmanagedMemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StackOnlyUnmanagedMemoryOwner<T> Allocate(int size) => new StackOnlyUnmanagedMemoryOwner<T>(size, false);

        /// <summary>
        /// Creates a new <see cref="StackOnlyUnmanagedMemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StackOnlyUnmanagedMemoryOwner<T> Allocate(int size, bool clear) => new StackOnlyUnmanagedMemoryOwner<T>(size, clear);

        /// <inheritdoc cref="MemoryMarshal.GetReference{T}(Span{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetReference()
        {
            return ref Unsafe.As<byte, T>(ref Buffer[0]);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }
}
