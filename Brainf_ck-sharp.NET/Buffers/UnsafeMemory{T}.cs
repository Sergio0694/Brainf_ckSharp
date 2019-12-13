using System;
using System.Runtime.CompilerServices;

namespace Brainf_ck_sharp.NET.Buffers
{
    /// <summary>
    /// A <see langword="struct"/> that maps a range of <typeparamref name="T"/> values on an existing buffer
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    internal readonly unsafe struct UnsafeMemory<T> where T : unmanaged
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
        /// Creates a new <see cref="UnsafeMemory{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="ptr"></param>
        public UnsafeMemory(int size, T* ptr)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "The size must be a positive number");

            Size = size;
            Ptr = ptr;
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> value at the specified index in the current buffer
        /// </summary>
        /// <param name="index">The target index to read the value from</param>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[index];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Ptr[index] = value;
        }

        /// <summary>
        /// Gets a new <see cref="UnsafeMemory{T}"/> instance representing a view over the current buffer
        /// </summary>
        /// <param name="start">The inclusive starting index for the new <see cref="UnsafeMemory{T}"/> instance</param>
        /// <param name="end">The exclusive ending index for the new <see cref="UnsafeMemory{T}"/> instance</param>
        /// <returns>A new <see cref="UnsafeMemory{T}"/> instance mapping values in the [start, end) range on the current buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeMemory<T> Slice(int start, int end) => new UnsafeMemory<T>(end - start, Ptr + start);
    }
}
