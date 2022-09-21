using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Models.Ide;
using CommunityToolkit.Diagnostics;

#nullable enable

namespace Brainf_ckSharp.Uwp.StyleSelectors;

/// <summary>
/// A style selector for the item containers of <see cref="CodeLibraryEntry"/> instances
/// </summary>
public sealed class CodeLibraryEntryItemContainerStyleSelector : StyleSelector
{
    /// <summary>
    /// Gets or sets the <see cref="Style"/> for the <see cref="ListViewItem"/> container for a <see cref="CodeLibraryEntry"/> instance
    /// </summary>
    public Style? DefaultContainerStyle { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Style"/> for the <see cref="ListViewItem"/> container for a placeholder item
    /// </summary>
    public Style? PlaceholderContainerStyle { get; set; }

    /// <inheritdoc/>
    protected override Style? SelectStyleCore(object item, DependencyObject container)
    {
        return item switch
        {
            CodeLibraryEntry _ => DefaultContainerStyle,
            CodeLibrarySection _ => PlaceholderContainerStyle,
            _ => ThrowHelper.ThrowArgumentNullException<Style>(nameof(item), "The input item can't be null")
        };
    }
}
