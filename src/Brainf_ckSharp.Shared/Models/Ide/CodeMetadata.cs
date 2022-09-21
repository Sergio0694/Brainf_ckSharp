namespace Brainf_ckSharp.Shared.Models.Ide;

/// <summary>
/// A model that contains metadata for a given source code
/// </summary>
public sealed class CodeMetadata
{
    /// <summary>
    /// Gets whether or not the current source code is favorited
    /// </summary>
    public bool IsFavorited { get; set; }

    /// <summary>
    /// Gets the default, empty <see cref="CodeMetadata"/> instance
    /// </summary>
    public static CodeMetadata Default { get; } = new();
}
