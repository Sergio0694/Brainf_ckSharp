using System;

namespace Brainf_ckSharp.Uwp.Enums.Abstract
{
    /// <summary>
    /// A simple <see langword="class"/> wrapping an <see cref="Enum"/> value
    /// </summary>
    /// <typeparam name="T">The <see cref="Enum"/> value to wrap</typeparam>
    public abstract class Box<T> where T : struct, Enum 
    {
        /// <summary>
        /// Creates a new <see cref="Box{T}"/> instance from the input value
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> value to use</param>
        protected Box(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the wrapped <typeparamref name="T"/> value for the current instance
        /// </summary>
        public T Value { get; }
    }
}
