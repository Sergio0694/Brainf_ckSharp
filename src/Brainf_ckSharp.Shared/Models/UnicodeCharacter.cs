namespace Brainf_ckSharp.Shared.Models;

/// <summary>
/// A simple model that represents a unicode character
/// </summary>
public sealed class UnicodeCharacter
{
    /// <summary>
    /// Creates a new <see cref="UnicodeCharacter"/> instance.
    /// </summary>
    /// <remarks>Needed to prevent the XAML compiler from producing invalid code.</remarks>
    internal UnicodeCharacter()
    {
    }

    /// <summary>
    /// Gets the character for the current instance
    /// </summary>
    /// <remarks>Not using <see langword="init"/> to prevent the XAML compiler from producing invalid code.</remarks>
    public required char Value { get; set; }
}
