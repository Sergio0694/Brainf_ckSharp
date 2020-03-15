using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Enums;
using Brainf_ckSharp.Uwp.Models.Ide;

#nullable enable

namespace Brainf_ckSharp.Uwp.TemplateSelectors
{
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
            return item switch
            {
                CodeLibraryEntry entry when entry.File.IsFromPackageDirectory() => SampleTemplate,
                CodeLibraryEntry _ => RecentItemTemplate,
                CodeLibrarySection c when c == CodeLibrarySection.Favorites => FavoritePlaceholderTemplate,
                CodeLibrarySection c when c == CodeLibrarySection.Recent => RecentPlaceholderTemplate,
                null => throw new ArgumentNullException(nameof(item), "The input item can't be null"),
                _ => throw new ArgumentException($"Unsupported item of type {item.GetType()}")
            } ?? throw new ArgumentException($"Missing template for item of type {item.GetType()}");
        }
    }
}
