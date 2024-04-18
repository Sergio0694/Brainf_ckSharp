namespace Brainf_ckSharp.Shared.Models;

/// <summary>
/// A simple model that represents a unicode character
/// </summary>
public sealed class UnicodeCharacter
{
    /// <summary>
    /// Gets the character for the current instance
    /// </summary>
    public required char Value { get; init; }
}
