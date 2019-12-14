using System.Runtime.CompilerServices;

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

        public StdoutBuffer() : base(StdoutBufferSizeLimit) { }

        /// <summary>
        /// Gets whether or not new characters can be written to the current instance
        /// </summary>
        public bool CanWrite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Position < StdoutBufferSizeLimit - 1;
        }

        /// <summary>
        /// Writes a new character to the current instance
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(char c) => Ptr[_Position++] = c;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe string ToString() => new string(Ptr, 0, _Position);
    }
}
