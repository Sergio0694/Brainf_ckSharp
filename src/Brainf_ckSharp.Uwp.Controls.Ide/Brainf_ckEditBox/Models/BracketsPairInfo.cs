namespace Brainf_ckSharp.Uwp.Controls.Ide.Models;

/// <summary>
/// A simple <see langword="struct"/> representing the coordinates of a brackets pair
/// </summary>
internal readonly struct BracketsPairInfo
{
    /// <summary>
    /// The offset of the left bracket
    /// </summary>
    public readonly int Start;

    /// <summary>
    /// The offset of the right bracket
    /// </summary>
    public readonly int End;

    /// <summary>
    /// Creates a new <see cref="BracketsPairInfo"/> instance with the specified parameters
    /// </summary>
    /// <param name="start">The offset of the left bracket</param>
    /// <param name="end">The offset of the right bracket</param>
    public BracketsPairInfo(int start, int end)
    {
        this.Start = start;
        this.End = end;
    }
}
