using System.Runtime.CompilerServices;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp.Models.Internal;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StackFrame(Range range) : this(range, range.Start) { }

    /// <summary>
    /// Creates a new <see cref="StackFrame"/> instance with the specified parameters
    /// </summary>
    /// <param name="range">The range of operators to execute</param>
    /// <param name="offset">The current offset during execution</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StackFrame(Range range, int offset)
    {
        Assert(offset >= range.Start);
        Assert(offset <= range.End);

        this.Range = range;
        this.Offset = offset;
    }

    /// <summary>
    /// Creates a new <see cref="StackFrame"/> instance with the specified offset
    /// </summary>
    /// <param name="offset">The current offset during execution</param>
    /// <returns>A <see cref="StackFrame"/> instance like the current one, but with a different offset</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StackFrame WithOffset(int offset) => new(this.Range, offset);
}
