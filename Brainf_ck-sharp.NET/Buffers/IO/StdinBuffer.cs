using System;
using System.Runtime.CompilerServices;

namespace Brainf_ck_sharp.NET.Buffers.IO
{
    /// <summary>
    /// A <see langword="class"/> that represents a memory area to be used as stdin buffer
    /// </summary>
    internal sealed class StdinBuffer
    {
        /// <summary>
        /// The underlying buffer to read characters from
        /// </summary>
        private readonly string Data;

        /// <summary>
        /// The current position in the underlying buffer to read from
        /// </summary>
        private int _Position;

        /// <summary>
        /// Creates a new <see cref="StdinBuffer"/> instance with the specified parameters
        /// </summary>
        /// <param name="data">The input data to use to read characters from</param>
        public StdinBuffer(string data) => Data = data;

        /// <summary>
        /// Gets whether or not there are characters left in the current instance
        /// </summary>
        public bool CanRead
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Data.Length > _Position;
        }

        /// <summary>
        /// Reads a new character from the current instance
        /// </summary>
        /// <returns>The next <see cref="char"/> present in the underlying buffer</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the current buffer is exhausted</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char Read() => Data[_Position++];
    }
}
