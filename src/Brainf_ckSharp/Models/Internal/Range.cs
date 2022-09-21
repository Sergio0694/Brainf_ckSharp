using System.Runtime.CompilerServices;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp.Models.Internal;

/// <summary>
/// A <see langword="struct"/> that represents an interval of indices in a given sequence
/// </summary>
internal readonly struct Range
{
    /// <summary>
    /// The starting index for the current instance
    /// </summary>
    public readonly int Start;

    /// <summary>
    /// The ending index for the current instance
    /// </summary>
    public readonly int End;

    /// <summary>
    /// Creates a new <see cref="Range"/> instance with the specified parameters
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Range(int start, int end)
    {
        Assert(start >= 0);
        Assert(end >= 0);
        Assert(start <= end);

        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the length of the current instance
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => End - Start;
    }
}
