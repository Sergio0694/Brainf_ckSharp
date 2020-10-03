using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Extensions;
using static System.Diagnostics.Debug;

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
        /// Creates a new <see cref="Writer"/> instance to write to the underlying buffer
        /// </summary>
        /// <returns>A <see cref="Writer"/> instance to write characters</returns>
        [Pure]
        public Writer CreateWriter()
        {
            return new Writer(Buffer, _Position);
        }

        /// <summary>
        /// Synchronizes the current buffer position with a given <see cref="Writer"/> instance
        /// </summary>
        /// <param name="writer">The <see cref="Writer"/> instance that was previously used</param>
        public void Synchronize(ref Writer writer)
        {
            _Position = writer.Position;
        }

        /// <summary>
        /// A <see langword="struct"/> that can writer data on the memory from a <see cref="StdoutBuffer"/> instance
        /// </summary>
        public ref struct Writer
        {
            /// <inheritdoc cref="StdoutBuffer.Buffer"/>
            private readonly Span<char> Buffer;

            /// <inheritdoc cref="_Position"/>
            public int Position;

            /// <summary>
            /// Creates a new <see cref="Writer"/> instance targeting the input buffer
            /// </summary>
            /// <param name="buffer">The input buffer to write to</param>
            /// <param name="position">The initial position to write from</param>
            public Writer(Span<char> buffer, int position = 0)
            {
                Buffer = buffer;
                Position = position;
            }

            /// <summary>
            /// Tries to write a new character into the current buffer
            /// </summary>
            /// <param name="c">The input character to write to the underlying buffer</param>
            /// <returns><see langword="true"/> if the character was written successfully, <see langword="false"/> otherwise</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryWrite(char c)
            {
                Assert(Position >= 0);
                Assert(Position <= Specs.StdoutBufferSizeLimit);

                int position = Position;

                if ((uint)position < (uint)Buffer.Length)
                {
                    Buffer.DangerousGetReferenceAt(position) = c;

                    Position = position + 1;

                    return true;
                }

                return false;
            }

            /// <inheritdoc/>
            public override readonly string ToString()
            {
                return StringPool.Shared.GetOrAdd(Buffer.Slice(0, Position));
            }
        }

        /// <inheritdoc/>
        public override readonly string ToString()
        {
            return StringPool.Shared.GetOrAdd(new ReadOnlySpan<char>(Buffer, 0, _Position));
        }

        /// <inheritdoc/>
        public readonly void Dispose()
        {
            ArrayPool<char>.Shared.Return(Buffer);
        }
    }
}
