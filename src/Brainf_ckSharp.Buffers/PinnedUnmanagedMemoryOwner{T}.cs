using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers
{
    /// <summary>
    /// A <see langword="class"/> that owns a memory buffer that can be used across stack frames
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    public unsafe class PinnedUnmanagedMemoryOwner<T> : IDisposable where T : unmanaged
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
        protected readonly T* Ptr;

        /// <summary>
        /// The <see cref="GCHandle"/> instance used to pin <see cref="Buffer"/>
        /// </summary>
        /// <remarks>This field is not <see langword="readonly"/> to prevent the safe copy when calling <see cref="GCHandle.Free"/> from <see cref="Dispose"/></remarks>
        private GCHandle _Handle;

        /// <summary>
        /// Creates an empty <see cref="PinnedUnmanagedMemoryOwner{T}"/> instance
        /// </summary>
        protected PinnedUnmanagedMemoryOwner() { }

        /// <summary>
        /// Creates a new <see cref="PinnedUnmanagedMemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        protected PinnedUnmanagedMemoryOwner(int size, bool clear)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(size, 0, nameof(size));

            Size = size;
            Buffer = ArrayPool<byte>.Shared.Rent(size * sizeof(T));
            _Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            Ptr = (T*)Unsafe.AsPointer(ref Buffer[0]);

            if (clear && Size > 0) new Span<T>(Ptr, Size).Clear();
        }

        /// <summary>
        /// Gets an empty <see cref="PinnedUnmanagedMemoryOwner{T}"/> instance
        /// </summary>
        public static PinnedUnmanagedMemoryOwner<T> Empty { get; } = new PinnedUnmanagedMemoryOwner<T>();

        /// <summary>
        /// Gets an <see cref="UnmanagedSpan{T}"/> instance mapping the values on the current buffer
        /// </summary>
        public UnmanagedSpan<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new UnmanagedSpan<T>(Size, Ptr);
        }

        /// <summary>
        /// Gets an <see cref="Span{T}"/> instance mapping the values on the current buffer
        /// </summary>
        public Span<T> CoreCLRSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Span<T>(Ptr, Size);
        }

        /// <summary>
        /// Gets an <see cref="Span{T}"/> instance mapping the values on the current buffer
        /// </summary>
        public ReadOnlySpan<T> CoreCLRReadOnlySpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new ReadOnlySpan<T>(Ptr, Size);
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
        /// Creates a new <see cref="PinnedUnmanagedMemoryOwner{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        /// <remarks>This method is just a proxy for the <see langword="protected"/> constructor, for clarity</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PinnedUnmanagedMemoryOwner<T> Allocate(int size, bool clear) => new PinnedUnmanagedMemoryOwner<T>(size, clear);

        /// <inheritdoc cref="MemoryMarshal.GetReference{T}(Span{T})"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetReference()
        {
            DebugGuard.MustBeGreaterThan(Size, 0, nameof(Size));

            return ref Unsafe.AsRef<T>(Ptr);
        }

        /// <summary>
        /// Returns a <typeparamref name="T"/><see langword="*"/> pointer to the underlying buffer
        /// </summary>
        /// <returns>A <typeparamref name="T"/><see langword="*"/> pointer to the underlying buffer</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetPointer()
        {
            DebugGuard.MustBeGreaterThan(Size, 0, nameof(Size));

            return Ptr;
        }

        /// <summary>
        /// Invokes <see cref="Dispose"/> to free the allocated resources when this instance is destroyed
        /// </summary>
        ~PinnedUnmanagedMemoryOwner() => Dispose();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_Handle.IsAllocated) return;

            _Handle.Free();
            ArrayPool<byte>.Shared.Return(Buffer);

            GC.SuppressFinalize(this);
        }
    }
}
