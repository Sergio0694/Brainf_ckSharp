using System;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance;

namespace Brainf_ckSharp.Git.Buffers
{
    /// <summary>
    /// A <see langword="class"/> that implements a pool of items of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of items to store in the pool</typeparam>
    /// <remarks>The items pool is not thread safe</remarks>
    public sealed class Pool<T> where T : class, new()
    {
        /// <summary>
        /// The minimum size of the items pool
        /// </summary>
        private const int MinimumPoolSize = 8;

        /// <summary>
        /// The current pool of items
        /// </summary>
        /// <remarks>Not using the array pool here since previous pools are no longer needed</remarks>
        private T[] _Items;

        /// <summary>
        /// The current offset into the pool of items
        /// </summary>
        private int _Offset;

        /// <summary>
        /// Creates a new <see cref="Pool{T}"/> instance.
        /// </summary>
        private Pool()
        {
            var items = _Items = new T[MinimumPoolSize];

            ref T r0 = ref items.DangerousGetReference();

            // Initialize the items in the pool
            for (int i = 0; i < MinimumPoolSize; i++)
            {
                Unsafe.Add(ref r0, i) = new T();
            }
        }

        /// <summary>
        /// Gets the default, shared <see cref="Pool{T}"/> instance.
        /// </summary>
        public static Pool<T> Shared { get; } = new Pool<T>();

        /// <summary>
        /// Rents a new <typeparamref name="T"/> instance
        /// </summary>
        /// <returns>A recycled <typeparamref name="T"/> instance to use</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Rent()
        {
            // Expand the current pool, if needed
            if (_Offset == _Items.Length)
            {
                ExpandBuffer();
            }

            return _Items.DangerousGetReferenceAt(_Offset++);
        }

        /// <summary>
        /// Expands the current buffer with pooled items
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExpandBuffer()
        {
            T[] oldItems = _Items;
            T[] newItems = new T[oldItems.Length * 2];

            // Copy over the previous elements
            oldItems.AsSpan().CopyTo(newItems);

            _Items = newItems;

            ref T r0 = ref newItems.DangerousGetReference();
            int end = newItems.Length;

            for (int i = _Offset; i < end; i++)
            {
                Unsafe.Add(ref r0, i) = new T();
            }
        }

        /// <summary>
        /// Resets the current pool of items
        /// </summary>
        /// <remarks>This can cause previously rented objects to be reused</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _Offset = 0;
    }
}
