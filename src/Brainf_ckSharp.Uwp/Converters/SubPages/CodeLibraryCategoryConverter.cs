using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="CodeLibrarySection"/> values
    /// </summary>
    public static class CodeLibraryCategoryConverter
    {
        /// <summary>
        /// Converts a <see cref="CodeLibrarySection"/> value into its representation
        /// </summary>
        /// <param name="section">The input <see cref="CodeLibrarySection"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="CodeLibrarySection"/> value</returns>
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
