using System.Diagnostics.Contracts;
using Brainf_ckSharp.Uwp.Resources;

namespace Brainf_ckSharp.Uwp.Bindings.Functions.Ide
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions for the favorite state of a source code entry
    /// </summary>
    public static class CodeLibraryEntryFavoriteStateFormatter
    {
        /// <summary>
        /// Formats the label of a favorite state
        /// </summary>
        /// <param name="flag">The favorite state</param>
        /// <returns>A <see cref="string"/> representing the input favorite state</returns>
        [Pure]
        public static string FormatLabel(bool flag)
        {
            return flag switch
            {
                true => "Remove from favorites",
                false => "Add to favorites"
            };
        }

        /// <summary>
        /// Formats the icon of a favorite state
        /// </summary>
        /// <param name="flag">The favorite state</param>
        /// <returns>A <see cref="string"/> representing the input favorite state</returns>
        [Pure]
        public static string FormatIcon(bool flag)
        {
            return flag switch
            {
                true => XamlResources.Icons.RemoveFromFavorites,
                false => XamlResources.Icons.AddToFavorites
            };
        }
    }
}
