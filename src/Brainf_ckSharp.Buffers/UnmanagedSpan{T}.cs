using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Brainf_ckSharp.Buffers
{
    /// <summary>
    /// A <see langword="struct"/> that maps a range of <typeparamref name="T"/> values on an existing buffer
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    public readonly unsafe struct UnmanagedSpan<T> where T : unmanaged
    {
        /// <summary>
        /// The size of the usable buffer for the current instance
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// A pointer to the first element in of the underlying buffer for the current instance
        /// </summary>
        private readonly T* Ptr;

        /// <summary>
        /// Creates a new <see cref="UnmanagedSpan{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="ptr"></param>
        public UnmanagedSpan(int size, T* ptr)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(size, 0, nameof(size));
            DebugGuard.MustBeFalse(ptr == null && size > 0, nameof(ptr));

            Size = size;
            Ptr = ptr;
        }

        /// <summary>
        /// Gets an empty <see cref="UnmanagedSpan{T}"/> instance
        /// </summary>
        public static UnmanagedSpan<T> Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new UnmanagedSpan<T>(0, null);
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> value at the specified index in the current buffer
        /// </summary>
        /// <param name="index">The target index to read the value from</param>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                DebugGuard.MustBeGreaterThanOrEqualTo(index, 0, nameof(index));
                DebugGuard.MustBeLessThan(index, Size, nameof(index));

                return ref Ptr[index];
            }
        }

        /// <summary>
        /// Gets a new <see cref="UnmanagedSpan{T}"/> instance representing a view over the current buffer
        /// </summary>
        /// <param name="start">The inclusive starting index for the new <see cref="UnmanagedSpan{T}"/> instance</param>
        /// <param name="end">The exclusive ending index for the new <see cref="UnmanagedSpan{T}"/> instance</param>
        /// <returns>A new <see cref="UnmanagedSpan{T}"/> instance mapping values in the [start, end) range on the current buffer</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnmanagedSpan<T> Slice(int start, int end)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(start, 0, nameof(start));
            DebugGuard.MustBeGreaterThanOrEqualTo(end, 0, nameof(end));
            DebugGuard.MustBeLessThan(start, Size, nameof(start));
            DebugGuard.MustBeLessThanOrEqualTo(end, Size, nameof(end));
            DebugGuard.MustBeLessThanOrEqualTo(start, end, nameof(start));

            return new UnmanagedSpan<T>(end - start, Ptr + start);
        }
    }
}
