namespace Brainf_ckSharp.Uwp.Controls.Ide.Models;

/// <summary>
/// A simple <see langword="struct"/> representing the coordinates and the size info on a column guide
/// </summary>
internal readonly struct ColumnGuideInfo
{
    /// <summary>
    /// The X coordinate of the column guide
    /// </summary>
    public readonly float X;

    /// <summary>
    /// The Y coordinate of the column guide
    /// </summary>
    public readonly float Y;

    /// <summary>
    /// The height of the column guide
    /// </summary>
    public readonly float Height;

    /// <summary>
    /// Creates a new <see cref="ColumnGuideInfo"/> instance with the specified parameters
    /// </summary>
    /// <param name="x">The line X position</param>
    /// <param name="y">The line Y position</param>
    /// <param name="height">The line height</param>
    public ColumnGuideInfo(float x, float y, float height)
    {
        this.X = x;
        this.Y = y;
        this.Height = height;
    }
}
