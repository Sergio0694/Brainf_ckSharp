using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// A reference type that wraps a value type of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The value type to box</typeparam>
    public sealed class Box<T> where T : struct
    {
        /// <summary>
        /// Creates a new <see cref="Box{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The initial <typeparamref name="T"/> value for the new instance</param>
        public Box(T value) => Value = value;

        /// <summary>
        /// Gets or sets the wrapped <typeparamref name="T"/> value for the current instance
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="Box{T}"/> instance for a given <typeparamref name="T"/> value
        /// </summary>
        /// <param name="value">The input <typeparamref name="T"/> value to box</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Box<T>(T value) => new Box<T>(value);

        /// <summary>
        /// Extracts a <typeparamref name="T"/> value from a given <see cref="Box{T}"/> instance
        /// </summary>
        /// <param name="box">The input <see cref="Box{T}"/> instance to unbox</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Box<T> box) => box.Value;
    }
}
