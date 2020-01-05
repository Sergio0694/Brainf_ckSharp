using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Buffers.Extensions.Types;

namespace System.Buffers
{
    /// <summary>
    /// A <see langword="struct"/> that owns a memory buffer shared from a pool
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    public readonly struct MemoryOwner<T>
    {
        /// <summary>
        /// The size of the usable buffer within <see cref="Buffer"/>
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// The <typeparamref name="T"/> array that constitutes the memory buffer for the current instance
        /// </summary>
        private readonly T[] Buffer;

        /// <summary>
        /// Creates a new <see cref="MemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        private MemoryOwner(int size)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(size, 0, nameof(size));

            Size = size;
            Buffer = ArrayPool<T>.Shared.Rent(size);
        }

        /// <summary>
        /// Creates a new <see cref="MemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="buffer">The existing buffer to use</param>
        private MemoryOwner(int size, T[] buffer)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(size, 0, nameof(size));
            DebugGuard.MustBeGreaterThanOrEqualTo(buffer.Length, size, nameof(buffer));

            Size = size;
            Buffer = buffer;
        }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> with the contents of the current <see cref="MemoryOwner{T}"/> instance
        /// </summary>
        public Span<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Buffer.AsSpan(0, Size);
        }

        /// <summary>
        /// Creates a new <see cref="UnmanagedMemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemoryOwner<T> Allocate(int size) => new MemoryOwner<T>(size);

        /// <inheritdoc cref="MemoryMarshal.GetReference{T}(Span{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetReference()
        {
            return ref Buffer[0];
        }

        /// <summary>
        /// Returns a <see cref="ReadOnlySpanEnumerator{T}"/> instance that enumerates the items in the current buffer
        /// </summary>
        /// <returns>A<see cref="ReadOnlySpanEnumerator{T}"/> instance that enumerates the items in the current buffer</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpanEnumerator<T> Enumerate() => new ReadOnlySpanEnumerator<T>(new ReadOnlySpan<T>(Buffer, 0, Size));

        /// <summary>
        /// Creates a new <see cref="MemoryOwner{T}"/> instance with a specified size over the current buffer
        /// </summary>
        /// <param name="size">The size of the new <see cref="MemoryOwner{T}"/> instance to create</param>
        /// <returns>A new <see cref="MemoryOwner{T}"/> instance with a specified size over the current buffer</returns>
        /// <remarks><see cref="Dispose"/> must never be called on <see cref="MemoryOwner{T}"/> instances pointing to the same buffer</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryOwner<T> Slice(int size)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(size, 0, nameof(size));
            DebugGuard.MustBeLessThanOrEqualTo(size, Size, nameof(size));

            return new MemoryOwner<T>(size, Buffer);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(Buffer);
        }
    }
}
