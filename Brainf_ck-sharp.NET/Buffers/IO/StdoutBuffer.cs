using System.Runtime.CompilerServices;

#pragma warning disable IDE0032

namespace Brainf_ck_sharp.NET.Buffers.IO
{
    /// <summary>
    /// A <see langword="class"/> that represents a memory area to be used as stdout buffer
    /// </summary>
    internal sealed class StdoutBuffer : UnsafeMemoryBuffer<char>
    {
        /// <summary>
        /// The maximum allowed size for the output buffer
        /// </summary>
        public const int StdoutBufferSizeLimit = 1024;

        /// <summary>
        /// The current position in the underlying buffer to write to
        /// </summary>
        private int _Position;

        public StdoutBuffer() : base(StdoutBufferSizeLimit, false) { }

        /// <summary>
        /// Gets the current length of the text in the output buffer
        /// </summary>
        public int Length => _Position;

        /// <summary>
        /// Tries to write a new character into the current buffer
        /// </summary>
        /// <param name="c">The input character to write to the underlying buffer</param>
        /// <returns><see langword="true"/> if the character was written successfully, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool TryWrite(char c)
        {
            if (_Position == StdoutBufferSizeLimit) return false;

            Ptr[_Position++] = c;
            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe string ToString() => new string(Ptr, 0, _Position);
    }
}
