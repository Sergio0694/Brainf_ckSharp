namespace Brainf_ckSharp.Shared.Models;

/// <summary>
/// A simple model that represents a unicode character
/// </summary>
public sealed class UnicodeCharacter
{
    /// <summary>
    /// Creates a new <see cref="UnicodeCharacter"/> instance with the specified parameters
    /// </summary>
    /// <param name="value">The input unicode character to wrap</param>
    public UnicodeCharacter(char value) => Value = value;

    /// <summary>
    /// Gets the character for the current instance
    /// </summary>
    public char Value { get; }
}
