using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ck_sharp.NET.Helpers;

namespace Brainf_ck_sharp.NET.Buffers
{
    /// <summary>
    /// A <see langword="struct"/> that owns a memory buffer shared from a common pool
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    internal ref struct UnsafeSpan<T> where T : unmanaged
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
        /// Creates a new <see cref="UnsafeSpan{T}"/> instaance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        private unsafe UnsafeSpan(int size)
        {
            DebugGuard.MustBeGreaterThan(size, 0, nameof(size));

            Size = size;
            Buffer = ArrayPool<byte>.Shared.Rent(size * sizeof(T));
        }

        /// <summary>
        /// Creates a new <see cref="UnsafeSpan{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeSpan<T> Allocate(int size) => new UnsafeSpan<T>(size);

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
