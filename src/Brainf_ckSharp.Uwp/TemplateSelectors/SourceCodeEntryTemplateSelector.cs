using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Models.Ide;
using Microsoft.Toolkit.Diagnostics;

#nullable enable

namespace Brainf_ckSharp.Uwp.TemplateSelectors;

/// <summary>
/// A template selector for source code entries
/// </summary>
public sealed class SourceCodeEntryTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the placeholder in the favorites section
    /// </summary>
    public DataTemplate? FavoritePlaceholderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the placeholder in the history section
    /// </summary>
    public DataTemplate? RecentPlaceholderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for a recent item
    /// </summary>
    public DataTemplate? RecentItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the code samples
    /// </summary>
    public DataTemplate? SampleTemplate { get; set; }

    /// <inheritdoc/>
    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        Guard.IsNotNull(item, nameof(item));

        DataTemplate? template = item switch
        {
            CodeLibraryEntry entry when entry.File.IsReadOnly => SampleTemplate,
            CodeLibraryEntry _ => RecentItemTemplate,
            CodeLibrarySection.Favorites => FavoritePlaceholderTemplate,
            CodeLibrarySection.Recent => RecentPlaceholderTemplate,
            _ => ThrowHelper.ThrowArgumentException<DataTemplate>("Invalid item type")
        };

        if (template is null)
        {
            ThrowHelper.ThrowInvalidOperationException("The requested template is null");
        }

        return template;
    }
}
