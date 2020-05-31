using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums;
using Microsoft.Toolkit.Collections;

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
        public static string ConvertSectionName(CodeLibrarySection section)
        {
            return section switch
            {
                CodeLibrarySection.Favorites => "Favorites",
                CodeLibrarySection.Recent => "Recent",
                CodeLibrarySection.Samples => "Samples",
                _ => throw new ArgumentException($"Invalid input value: {section}", nameof(section))
            };
        }

        /// <summary>
        /// Converts a group into its description for a given section
        /// </summary>
        /// <param name="group">The input <see cref="IReadOnlyObservableGroup"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="IReadOnlyObservableGroup"/> instance</returns>
        [Pure]
        public static string ConvertSectionDescription(IReadOnlyObservableGroup group)
        {
            return (CodeLibrarySection)group.Key switch
            {
                CodeLibrarySection.Favorites => group.Count > 0
                    ? $"{group.Count} favorite script{(group.Count > 1 ? "s" : string.Empty)}"
                    : "No favorite scripts",
                CodeLibrarySection.Recent => group.Count > 0
                    ? $"{group.Count} recent script{(group.Count > 1 ? "s" : string.Empty)}"
                    : "No recent scripts",
                CodeLibrarySection.Samples => group.Count > 0
                    ? $"{group.Count} sample script{(group.Count > 1 ? "s" : string.Empty)}"
                    : "No sample scripts",
                _ => throw new ArgumentException($"Invalid group value: {group.Key}", nameof(group))
            };
        }
    }
}
