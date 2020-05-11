using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;
using Microsoft.Toolkit.HighPerformance.Extensions;

#nullable enable

namespace Brainf_ckSharp.Buffers
{
    /// <summary>
    /// A <see langword="class"/> that represents a memory area to be used as stdout buffer
    /// </summary>
    internal sealed class StdoutBuffer : IDisposable
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
        private readonly char[]? Buffer;

        /// <summary>
        /// The current position in the underlying buffer to write to
        /// </summary>
        private int _Position;

        /// <summary>
        /// Creates a new <see cref="StdoutBuffer"/> instance
        /// </summary>
        public StdoutBuffer()
        {
            Buffer = ArrayPool<char>.Shared.Rent(Specs.StdoutBufferSizeLimit);
        }

        /// <summary>
        /// Tries to write a new character into the current buffer
        /// </summary>
        /// <param name="c">The input character to write to the underlying buffer</param>
        /// <returns><see langword="true"/> if the character was written successfully, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryWrite(char c)
        {
            if (_Position == Specs.StdoutBufferSizeLimit) return false;

            Debug.Assert(Buffer != null);
            Debug.Assert(_Position >= 0);
            Debug.Assert(_Position < Specs.StdoutBufferSizeLimit);

            Buffer!.DangerousGetReferenceAt(_Position++) = c;

            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => new string(Buffer, 0, _Position);

        /// <inheritdoc/>
        public void Dispose()
        {
#if DEBUG
            // Avoid adding a field to indicate whether the instance has been disposed.
            // As an additional check, when running in DEBUG mode just check whether
            // the rented array is null, otherwise return it and then override
            // the field to set it to null. If this method is ever called twice
            // by accident, this hack will make sure the unit tests will fail.
            Debug.Assert(Buffer != null);

            ArrayPool<char>.Shared.Return(Buffer);

            Unsafe.AsRef(Buffer) = null;
#else
            ArrayPool<char>.Shared.Return(Buffer);
#endif
        }
    }
}
