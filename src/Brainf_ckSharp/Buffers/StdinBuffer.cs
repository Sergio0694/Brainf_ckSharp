using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp.Buffers;

/// <summary>
/// A <see langword="struct"/> that represents a memory area to be used as stdin buffer
/// </summary>
/// <param name="data">The input data to use to read characters from</param>
internal struct StdinBuffer(ReadOnlyMemory<char> data)
{
    /// <summary>
    /// The underlying buffer to read characters from
    /// </summary>
    private readonly ReadOnlyMemory<char> data = data;

    /// <summary>
    /// The current position in the underlying buffer to read from
    /// </summary>
    private int position = 0;

    /// <summary>
    /// Creates a new <see cref="Reader"/> instance to read from the underlying buffer
    /// </summary>
    /// <returns>A <see cref="Reader"/> instance to read characters</returns>
    public readonly Reader CreateReader()
    {
        return new(this.data.Span, this.position);
    }

    /// <summary>
    /// Synchronizes the current buffer position with a given <see cref="Reader"/> instance
    /// </summary>
    /// <param name="reader">The <see cref="Reader"/> instance that was previously used</param>
    public void Synchronize(ref Reader reader)
    {
        this.position = reader.Position;
    }

    /// <summary>
    /// A <see langword="struct"/> that can read data from memory from a <see cref="StdinBuffer"/> instance
    /// </summary>
    /// <param name="data">The input buffer to read from</param>
    /// <param name="position">The initial position to read from</param>
    public ref struct Reader(ReadOnlySpan<char> data, int position = 0)
    {
        /// <inheritdoc cref="StdinBuffer.data"/>
        private readonly ReadOnlySpan<char> data = data;

        /// <inheritdoc cref="position"/>
        public int Position = position;

        /// <summary>
        /// Tries to read a character from the current buffer
        /// </summary>
        /// <param name="c">The resulting character to read from the underlying buffer</param>
        /// <returns><see langword="true"/> if the character was read successfully, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRead(out char c)
        {
            Assert(this.Position >= 0);
            Assert(this.Position <= this.data.Length);

            int position = this.Position;

            if ((uint)position < (uint)this.data.Length)
            {
                c = this.data.DangerousGetReferenceAt(position);

                this.Position = position + 1;

                return true;
            }

            c = default;

            return false;
        }

        /// <inheritdoc/>
        public override readonly string ToString()
        {
            return StringPool.Shared.GetOrAdd(this.data);
        }
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        // If the underlying data belongs to a string and the range is its entire length,
        // we can just return that same instance with no additional allocations (this is the
        // same behavior of ReadOnlyMemory<char>.ToString()). Otherwise, we use StringPool to
        // avoid repeated allocations if the source buffers represent a repeated text.
        if (MemoryMarshal.TryGetString(this.data, out string text, out int start, out int length) &&
            start == 0 && length == text.Length)
        {
            return text;
        }

        return StringPool.Shared.GetOrAdd(this.data.Span);
    }
}
