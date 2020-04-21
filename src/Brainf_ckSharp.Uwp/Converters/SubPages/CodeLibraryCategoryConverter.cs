using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="CodeLibrarySection"/> instances
    /// </summary>
    public static class CodeLibraryCategoryConverter
    {
        /// <summary>
        /// Converts a <see cref="CodeLibrarySection"/> instance into its representation
        /// </summary>
        /// <param name="section">The input <see cref="CodeLibrarySection"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="CodeLibrarySection"/> instance</returns>
        [Pure]
        public static string Convert(CodeLibrarySection section)
        {
            if (section == CodeLibrarySection.Favorites) return "Favorites";
            if (section == CodeLibrarySection.Recent) return "Recent";
            if (section == CodeLibrarySection.Samples) return "Samples";

            throw new ArgumentException($"Invalid input value: {section}", nameof(section));
        }
    }
}
