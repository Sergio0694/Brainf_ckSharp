using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums;
using Microsoft.Toolkit.Diagnostics;

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
            Guard.IsNotNull(item, nameof(item));

            DataTemplate? template = item switch
            {
                UserGuideSection.Introduction => IntroductionTemplate,
                UserGuideSection.Debugging => DebuggingTemplate,
                UserGuideSection.PBrain => PBrainTemplate,
                UserGuideSection.Samples => CodeSamplesTemplate,
                _ => ThrowHelper.ThrowArgumentException<DataTemplate>("Invalid item type")
            };

            if (template is null)
            {
                ThrowHelper.ThrowInvalidOperationException("The requested template is null");
            }

            return template;
        }
    }
}
