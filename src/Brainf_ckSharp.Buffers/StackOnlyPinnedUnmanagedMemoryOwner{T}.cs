using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers
{
    /// <summary>
    /// A <see langword="struct"/> that owns a memory buffer that can be used across stack frames
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    /// <remarks>This type mirrors <see cref="PinnedUnmanagedMemoryOwner{T}"/>, but as a value type</remarks>
    public unsafe ref struct StackOnlyPinnedUnmanagedMemoryOwner<T> where T : unmanaged
    {
        /// <summary>
        /// The size of the usable buffer within <see cref="Buffer"/>
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// The <see cref="byte"/> array that constitutes the memory buffer for the current instance
        /// </summary>
        private readonly byte[]? Buffer;

        /// <summary>
        /// A pointer to the first element in <see cref="Buffer"/>
        /// </summary>
        private readonly T* Ptr;

        /// <summary>
        /// The <see cref="GCHandle"/> instance used to pin <see cref="Buffer"/>
        /// </summary>
        /// <remarks>This field is not <see langword="readonly"/> to prevent the safe copy when calling <see cref="GCHandle.Free"/> from <see cref="Dispose"/></remarks>
        private GCHandle _Handle;

        /// <summary>
        /// Creates a new <see cref="StackOnlyPinnedUnmanagedMemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        /// <remarks>Not using a proxy like <see cref="PinnedUnmanagedMemoryOwner{T}.Allocate"/> here since it's a value type</remarks>
        private StackOnlyPinnedUnmanagedMemoryOwner(int size, bool clear)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(size, 0, nameof(size));

            Size = size;
            Buffer = ArrayPool<byte>.Shared.Rent(size * sizeof(T));
            _Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            Ptr = (T*)Unsafe.AsPointer(ref Buffer[0]);

            if (clear && Size > 0) new Span<T>(Ptr, Size).Clear();
        }

        /// <summary>
        /// Gets an <see cref="UnmanagedSpan{T}"/> instance mapping the values on the current buffer
        /// </summary>
        public UnmanagedSpan<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new UnmanagedSpan<T>(Size, Ptr);
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
        /// Creates a new <see cref="StackOnlyPinnedUnmanagedMemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StackOnlyPinnedUnmanagedMemoryOwner<T> Allocate(int size, bool clear) => new StackOnlyPinnedUnmanagedMemoryOwner<T>(size, clear);

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            DebugGuard.MustBeTrue(_Handle.IsAllocated, nameof(_Handle));

            _Handle.Free();
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }
}
