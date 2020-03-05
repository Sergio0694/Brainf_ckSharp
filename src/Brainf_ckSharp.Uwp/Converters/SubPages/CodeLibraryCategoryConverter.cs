using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Uwp.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="CodeLibraryCategory"/> instances
    /// </summary>
    public static class CodeLibraryCategoryConverter
    {
        /// <summary>
        /// Converts a <see cref="CodeLibraryCategory"/> instance into its representation
        /// </summary>
        /// <param name="category">The input <see cref="CodeLibraryCategory"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="CodeLibraryCategory"/> instance</returns>
        [Pure]
        public static string Convert(CodeLibraryCategory category)
        {
            if (category == CodeLibraryCategory.Favorites) return "Favorites";
            if (category == CodeLibraryCategory.Recent) return "Recent";
            if (category == CodeLibraryCategory.Samples) return "Samples";

            throw new ArgumentException($"Invalid input value: {category}", nameof(category));
        }
    }
}
