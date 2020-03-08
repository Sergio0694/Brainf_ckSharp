using System.Buffers;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;

#pragma warning disable IDE0032

namespace Brainf_ckSharp.Buffers
{
    /// <summary>
    /// A <see langword="class"/> that represents a memory area to be used as stdout buffer
    /// </summary>
    internal sealed class StdoutBuffer : PinnedUnmanagedMemoryOwner<char>
    {
        /// <summary>
        /// The current position in the underlying buffer to write to
        /// </summary>
        private int _Position;

        public StdoutBuffer() : base(Specs.StdoutBufferSizeLimit, false) { }

        /// <summary>
        /// Tries to write a new character into the current buffer
        /// </summary>
        /// <param name="c">The input character to write to the underlying buffer</param>
        /// <returns><see langword="true"/> if the character was written successfully, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool TryWrite(char c)
        {
            if (_Position == Specs.StdoutBufferSizeLimit) return false;

            Ptr[_Position++] = c;

            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe string ToString() => new string(Ptr, 0, _Position);
    }
}
