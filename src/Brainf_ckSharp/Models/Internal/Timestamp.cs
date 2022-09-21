using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Brainf_ckSharp.Models.Internal;

/// <summary>
/// A model for a high resolution timestamp
/// </summary>
/// <remarks>
/// This can be used as a replacement for <see cref="Stopwatch"/> to avoid
/// the heap allocation when it's only used to calculate the elapsed time.
/// </remarks>
internal readonly struct Timestamp
{
    /// <summary>
    /// The frequency of the high performance ticks with respect to system ticks
    /// </summary>
    private static readonly double TickFrequency = 10_000_000d / Stopwatch.Frequency;

    /// <summary>
    /// The high resolution ticks for the current <see cref="Timestamp"/> value
    /// </summary>
    private readonly long Value;

    /// <summary>
    /// Creates a new <see cref="Timestamp"/> value with the specified parameters
    /// </summary>
    /// <param name="value">The high resolution ticks for the new value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Timestamp(long value)
    {
        Value = value;
    }

    /// <summary>
    /// Captures a <see cref="Timestamp"/> value representing the current instant
    /// </summary>
    public static Timestamp Now
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Stopwatch.GetTimestamp());
    }

    /// <summary>
    /// Gets the number of elapsed ticks from the current <see cref="Timestamp"/> value
    /// </summary>
    public long Ticks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            long
                current = Stopwatch.GetTimestamp(),
                delta = Math.Abs(current - Value),
                ticks = unchecked((long)(delta * TickFrequency));

            return ticks;
        }
    }
}
