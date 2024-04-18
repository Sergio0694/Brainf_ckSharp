namespace Brainf_ckSharp.Shared.Models;

/// <summary>
/// A simple model representing a range of unicode characters
/// </summary>
public sealed class UnicodeInterval
{
    /// <summary>
    /// Gets the start of the interval
    /// </summary>
    public required int Start { get; init; }

    /// <summary>
    /// Gets the end of the interval
    /// </summary>
    public required int End { get; init; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{Start}, {End}]";
    }
}
