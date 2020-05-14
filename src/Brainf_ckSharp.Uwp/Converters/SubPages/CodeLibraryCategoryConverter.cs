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
            return section switch
            {
                CodeLibrarySection.Favorites => "Favorites",
                CodeLibrarySection.Recent => "Recent",
                CodeLibrarySection.Samples => "Samples",
                _ => throw new ArgumentException($"Invalid input value: {section}", nameof(section))
            };
        }
    }
}
