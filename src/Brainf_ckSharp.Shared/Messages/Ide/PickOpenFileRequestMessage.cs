namespace Brainf_ckSharp.Shared.Messages.Ide;

/// <summary>
/// A message that signals whenever the user requests to pick and open a file
/// </summary>
/// <param name="favorite">Whether or not to add the picked file to the favorites</param>
public sealed class PickOpenFileRequestMessage(bool favorite)
{
    /// <summary>
    /// Gets whether or not to add the picked file to the favorites
    /// </summary>
    public bool Favorite { get; } = favorite;
}
