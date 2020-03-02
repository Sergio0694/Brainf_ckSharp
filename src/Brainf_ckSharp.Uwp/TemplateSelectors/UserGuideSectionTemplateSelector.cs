using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Enums;

#nullable enable

namespace Brainf_ckSharp.Uwp.TemplateSelectors
{
    /// <summary>
    /// A template selector for user guide sections
    /// </summary>
    public sealed class UserGuideSectionTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the introduction section
        /// </summary>
        public DataTemplate? IntroductionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the code samples section
        /// </summary>
        public DataTemplate? CodeSamplesTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the PBrain section
        /// </summary>
        public DataTemplate? PBrainTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the debugging guide section
        /// </summary>
        public DataTemplate? DebuggingTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                UserGuideSection s when s == UserGuideSection.Introduction => IntroductionTemplate,
                UserGuideSection s when s == UserGuideSection.Debugging => DebuggingTemplate,
                UserGuideSection s when s == UserGuideSection.PBrain => PBrainTemplate,
                UserGuideSection s when s == UserGuideSection.Samples => CodeSamplesTemplate,
                null => throw new ArgumentNullException(nameof(item), "The input item can't be null"),
                _ => throw new ArgumentException($"Unsupported item of type {item.GetType()}")
            } ?? throw new ArgumentException($"Missing template for item of type {item.GetType()}");
        }
    }
}
