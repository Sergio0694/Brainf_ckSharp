using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Models.Ide;
using Brainf_ckSharp.Uwp.Resources;

namespace Brainf_ckSharp.Uwp.Converters.SubPages;

/// <summary>
/// A <see langword="class"/> with a collection of helper functions for <see cref="CodeMetadata"/> instances
/// </summary>
public static class CodeMetadataConverter
{
    /// <summary>
    /// Converts the favorite state of a given <see cref="CodeMetadata"/> instance into a display label
    /// </summary>
    /// <param name="metadata">The input <see cref="CodeMetadata"/> instance</param>
    /// <returns>A <see cref="string"/> representing the input favorite state</returns>
    [Pure]
    public static string ConvertFavoriteLabel(CodeMetadata metadata)
    {
        return metadata.IsFavorited switch
        {
            true => "Remove from favorites",
            false => "Add to favorites"
        };
    }

    /// <summary>
    /// Converts the favorite state of a given <see cref="CodeMetadata"/> instance into an icon
    /// </summary>
    /// <param name="metadata">The input <see cref="CodeMetadata"/> instance</param>
    /// <returns>A <see cref="string"/> representing the input favorite state</returns>
    [Pure]
    public static string ConvertFavoriteIcon(CodeMetadata metadata)
    {
        return metadata.IsFavorited switch
        {
            true => XamlResources.Icons.RemoveFromFavorites,
            false => XamlResources.Icons.AddToFavorites
        };
    }
}
