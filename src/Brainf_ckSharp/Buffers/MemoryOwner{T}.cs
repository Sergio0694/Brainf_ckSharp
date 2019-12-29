using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Helpers;

namespace Brainf_ckSharp.Buffers
{
    /// <summary>
    /// A <see langword="struct"/> that owns a memory buffer shared from a common pool
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    public readonly struct MemoryOwner<T> where T : unmanaged
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
        /// Creates a new <see cref="MemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        private unsafe MemoryOwner(int size)
        {
            DebugGuard.MustBeGreaterThan(size, 0, nameof(size));

            Size = size;
            Buffer = ArrayPool<byte>.Shared.Rent(size * sizeof(T));
        }

        /// <summary>
        /// Creates a new <see cref="MemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="buffer">The existing buffer to use</param>
        private unsafe MemoryOwner(int size, byte[] buffer)
        {
            DebugGuard.MustBeGreaterThan(size, 0, nameof(size));
            DebugGuard.MustBeGreaterThanOrEqualTo(buffer.Length, size * sizeof(T), nameof(buffer));

            Size = size;
            Buffer = buffer;
        }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> with the contents of the current <see cref="MemoryOwner{T}"/> instance
        /// </summary>
        public Span<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.Cast<byte, T>(AsBytes());
        }

        /// <summary>
        /// Creates a new <see cref="MemoryOwner{T}"/> instance with the specified parameters
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
            return ref Unsafe.As<byte, T>(ref Buffer[0]);
        }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> of type <see cref="byte"/> with the contents of the current <see cref="MemoryOwner{T}"/> instance
        /// </summary>
        /// <returns>A a <see cref="Span{T}"/> of type <see cref="byte"/> with the contents of the current <see cref="MemoryOwner{T}"/> instance</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Span<byte> AsBytes() => new Span<byte>(Buffer, 0, Size * sizeof(T));

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
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }
}
