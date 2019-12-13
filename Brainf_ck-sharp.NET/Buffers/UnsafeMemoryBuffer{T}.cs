using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        protected readonly int Size;

        /// <summary>
        /// The <typeparamref name="T"/> array that constitutes the memory buffer for the current instance
        /// </summary>
        private readonly T[] Buffer;

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
        /// Creates a new <see cref="UnsafeMemoryBuffer{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        protected UnsafeMemoryBuffer(int size)
        {
            Size = size;
            Buffer = ArrayPool<T>.Shared.Rent(size);
            _Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            Ptr = (T*)Unsafe.AsPointer(ref Buffer[0]);
        }

        /// <summary>
        /// Creates a new <see cref="UnsafeMemoryBuffer{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        /// <remarks>This method is just a proxy for the <see langword="protected"/> constructor, for clarity</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeMemoryBuffer<T> Allocate(int size) => new UnsafeMemoryBuffer<T>(size);

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
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[index];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Ptr[index] = value;
        }

        /// <summary>
        /// Invokes <see cref="Dispose"/> to free the aallocated resources when this instance is destroyed
        /// </summary>
        ~UnsafeMemoryBuffer() => Dispose();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_Handle.IsAllocated) return;

            _Handle.Free();
            ArrayPool<T>.Shared.Return(Buffer);

            GC.SuppressFinalize(this);
        }
    }
}
