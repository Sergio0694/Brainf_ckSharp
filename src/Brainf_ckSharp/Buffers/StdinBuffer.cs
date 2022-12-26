using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp.Buffers;

/// <summary>
/// A <see langword="struct"/> that represents a memory area to be used as stdin buffer
/// </summary>
internal struct StdinBuffer
{
    /// <summary>
    /// The underlying buffer to read characters from
    /// </summary>
    private readonly ReadOnlyMemory<char> Data;

    /// <summary>
    /// The current position in the underlying buffer to read from
    /// </summary>
    private int _Position;

    /// <summary>
    /// Creates a new <see cref="StdinBuffer"/> instance with the specified parameters
    /// </summary>
    /// <param name="data">The input data to use to read characters from</param>
    public StdinBuffer(ReadOnlyMemory<char> data)
    {
        Data = data;
        _Position = 0;
    }

    /// <summary>
    /// Creates a new <see cref="Reader"/> instance to read from the underlying buffer
    /// </summary>
    /// <returns>A <see cref="Reader"/> instance to read characters</returns>
    [Pure]
    public Reader CreateReader()
    {
        return new(Data.Span, _Position);
    }

    /// <summary>
    /// Synchronizes the current buffer position with a given <see cref="Reader"/> instance
    /// </summary>
    /// <param name="reader">The <see cref="Reader"/> instance that was previously used</param>
    public void Synchronize(ref Reader reader)
    {
        _Position = reader.Position;
    }

    /// <summary>
    /// A <see langword="struct"/> that can read data from memory from a <see cref="StdinBuffer"/> instance
    /// </summary>
    public ref struct Reader
    {
        /// <inheritdoc cref="StdinBuffer.Data"/>
        private readonly ReadOnlySpan<char> Data;

        /// <inheritdoc cref="_Position"/>
        public int Position;

        /// <summary>
        /// Creates a new <see cref="Reader"/> instance targeting the input buffer
        /// </summary>
        /// <param name="data">The input buffer to read from</param>
        /// <param name="position">The initial position to read from</param>
        public Reader(ReadOnlySpan<char> data, int position = 0)
        {
            Data = data;
            Position = position;
        }

        /// <summary>
        /// Tries to read a character from the current buffer
        /// </summary>
        /// <param name="c">The resulting character to read from the underlying buffer</param>
        /// <returns><see langword="true"/> if the character was read successfully, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRead(out char c)
        {
            Assert(Position >= 0);
            Assert(Position <= Data.Length);

            int position = Position;

            if ((uint)position < (uint)Data.Length)
            {
                c = Data.DangerousGetReferenceAt(position);

                Position = position + 1;

                return true;
            }

            c = default;

            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return StringPool.Shared.GetOrAdd(Data);
        }
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        // If the underlying data belongs to a string and the range is its entire length,
        // we can just return that same instance with no additional allocations (this is the
        // same behavior of ReadOnlyMemory<char>.ToString()). Otherwise, we use StringPool to
        // avoid repeated allocations if the source buffers represent a repeated text.
        if (MemoryMarshal.TryGetString(Data, out string text, out int start, out int length) &&
            start == 0 && length == text.Length)
        {
            return text;
        }

        return StringPool.Shared.GetOrAdd(Data.Span);
    }
}
