using System.Runtime.CompilerServices;

namespace Brainf_ckSharp.Buffers
{
    /// <summary>
    /// A <see langword="struct"/> that represents a memory area to be used as stdin buffer
    /// </summary>
    internal struct StdinBuffer
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
        public StdinBuffer(string data)
        {
            Data = data;
            _Position = 0;
        }

        /// <summary>
        /// Tries to read a character from the current buffer
        /// </summary>
        /// <param name="c">The resulting character to read from the underlying buffer</param>
        /// <returns><see langword="true"/> if the character was read successfully, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRead(out char c)
        {
            if (_Position < Data.Length)
            {
                c = Data[_Position++];

                return true;
            }

            c = default;

            return false;
        }

        /// <inheritdoc/>
        public override string ToString() => Data;
    }
}
