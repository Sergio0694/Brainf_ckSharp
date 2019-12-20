using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ck_sharp.NET.Helpers;

namespace Brainf_ck_sharp.NET.Buffers
{
    /// <summary>
    /// A <see langword="class"/> that owns a memory buffer that can be used across stack frames
    /// </summary>
    /// <typeparam name="T">The type of items stored in the underlying buffer</typeparam>
    internal unsafe class UnsafeMemoryBuffer<T> : IDisposable where T : unmanaged
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
        /// Creates an empty <see cref="UnsafeMemoryBuffer{T}"/> instance
        /// </summary>
        protected UnsafeMemoryBuffer() { }

        /// <summary>
        /// Creates a new <see cref="UnsafeMemoryBuffer{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        protected UnsafeMemoryBuffer(int size, bool clear)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(size, 0, nameof(size));

            Size = size;
            Buffer = ArrayPool<byte>.Shared.Rent(size * sizeof(T));
            _Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            Ptr = (T*)Unsafe.AsPointer(ref Buffer[0]);

            if (clear && Size > 0) new Span<T>(Ptr, Size).Clear();
        }

        /// <summary>
        /// Creates a new <see cref="UnsafeMemoryBuffer{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <param name="clear">Indicates whether or not to clear the allocated memory area</param>
        /// <remarks>This method is just a proxy for the <see langword="protected"/> constructor, for clarity</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeMemoryBuffer<T> Allocate(int size, bool clear) => new UnsafeMemoryBuffer<T>(size, clear);

        /// <summary>
        /// Gets an empty <see cref="UnsafeMemoryBuffer{T}"/> instance
        /// </summary>
        public static UnsafeMemoryBuffer<T> Empty { get; } = new UnsafeMemoryBuffer<T>();

        /// <summary>
        /// Gets an <see cref="UnsafeMemory{T}"/> instance mapping the values on the current buffer
        /// </summary>
        public UnsafeMemory<T> Memory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new UnsafeMemory<T>(Size, Ptr);
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
        /// Invokes <see cref="Dispose"/> to free the allocated resources when this instance is destroyed
        /// </summary>
        ~UnsafeMemoryBuffer() => Dispose();

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
