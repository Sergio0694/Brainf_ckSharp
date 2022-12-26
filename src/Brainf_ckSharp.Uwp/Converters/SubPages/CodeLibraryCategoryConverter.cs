using Brainf_ckSharp.Shared.Enums;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Collections;

namespace Brainf_ckSharp.Uwp.Converters.SubPages;

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
    public static string ConvertSectionName(CodeLibrarySection section)
    {
        return section switch
        {
            CodeLibrarySection.Favorites => "Favorites",
            CodeLibrarySection.Recent => "Recent",
            CodeLibrarySection.Samples => "Samples",
            _ => ThrowHelper.ThrowArgumentException<string>(nameof(section), "Invalid target section")
        };
    }

    /// <summary>
    /// Converts a group into its description for a given section
    /// </summary>
    /// <param name="group">The input <see cref="IReadOnlyObservableGroup"/> instance</param>
    /// <returns>A <see cref="string"/> representing the input <see cref="IReadOnlyObservableGroup"/> instance</returns>
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
            _ => ThrowHelper.ThrowArgumentException<string>(nameof(group), "Invalid group value")
        };
    }
}
