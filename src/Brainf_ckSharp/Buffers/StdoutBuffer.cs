using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;

namespace Brainf_ckSharp.Buffers
{
    /// <summary>
    /// A <see langword="class"/> that represents a memory area to be used as stdout buffer
    /// </summary>
    internal struct StdoutBuffer : IDisposable
    {
        /// <summary>
        /// The underlying <see cref="char"/> buffer
        /// </summary>
        /// <remarks>
        /// This array is rented and disposed directly within this type to avoid
        /// having to use another reference type as container, which would have caused
        /// a third indirect reference to access any individual value in the buffer.
        /// Storing the buffer directly in this type exchanges some verbosity for speed.
        /// </remarks>
        private readonly char[] Buffer;

        /// <summary>
        /// The current position in the underlying buffer to write to
        /// </summary>
        private int _Position;

        /// <summary>
        /// Creates a new <see cref="StdoutBuffer"/> instance
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private StdoutBuffer(char[] buffer)
        {
            Buffer = buffer;
            _Position = 0;
        }

        /// <summary>
        /// Creates a new <see cref="StdoutBuffer"/> with an empty underlying buffer
        /// </summary>
        /// <returns>A new <see cref="StdoutBuffer"/> value ready to use</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StdoutBuffer Allocate()
        {
            return new StdoutBuffer(ArrayPool<char>.Shared.Rent(Specs.StdoutBufferSizeLimit));
        }

        /// <summary>
        /// Tries to write a new character into the current buffer
        /// </summary>
        /// <param name="c">The input character to write to the underlying buffer</param>
        /// <returns><see langword="true"/> if the character was written successfully, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryWrite(char c)
        {
            Debug.Assert(_Position >= 0);
            Debug.Assert(_Position <= Specs.StdoutBufferSizeLimit);

            char[] buffer = Buffer!;
            int position = _Position;

            // Like in the other buffer, manually check for bounds to remove the
            // automatic bounds check added by the JIT compiler in the array access.
            if ((uint)position < (uint)buffer.Length)
            {
                buffer[position] = c;

                _Position = position + 1;

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly string ToString() => new string(Buffer, 0, _Position);

        /// <inheritdoc/>
        public readonly void Dispose()
        {
            ArrayPool<char>.Shared.Return(Buffer);
        }
    }
}
