using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Extensions.Types;

namespace Brainf_ckSharp.Models.Internal
{
    /// <summary>
    /// A <see langword="struct"/> that represents a stack frame for the interpreter
    /// </summary>
    internal readonly struct StackFrame
    {
        /// <summary>
        /// The <see cref="Range"/> instance that indicates the operators to execute in the current stack frame
        /// </summary>
        public readonly Range Range;

        /// <summary>
        /// The operator offset for the current stack frame
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// Creates a new <see cref="StackFrame"/> instance with the specified parameters
        /// </summary>
        /// <param name="range">The range of operators to execute</param>
        public StackFrame(Range range) : this(range, range.Start) { }

        /// <summary>
        /// Creates a new <see cref="StackFrame"/> instance with the specified parameters
        /// </summary>
        /// <param name="range">The range of operators to execute</param>
        /// <param name="offset">The current offset during execution</param>
        public StackFrame(Range range, int offset)
        {
            Debug.Assert(offset >= range.Start);
            Debug.Assert(offset <= range.End);

            Range = range;
            Offset = offset;
        }

        /// <summary>
        /// Creates a new <see cref="StackFrame"/> instance with the specified offset
        /// </summary>
        /// <param name="offset">The current offset during execution</param>
        /// <returns>A <see cref="StackFrame"/> instance like the current one, but with a different offset</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StackFrame WithOffset(int offset) => new StackFrame(Range, offset);
    }
}
