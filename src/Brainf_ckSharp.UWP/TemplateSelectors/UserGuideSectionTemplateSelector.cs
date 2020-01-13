using System;
using System.Diagnostics;
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
            DataTemplate? template = null;

            if (item == UserGuideSection.Introduction) template = IntroductionTemplate;
            else if (item == UserGuideSection.Debugging) template = PBrainTemplate;
            else if (item == UserGuideSection.PBrain) template = PBrainTemplate;
            else if (item == UserGuideSection.Samples) template = CodeSamplesTemplate;

            if (!(template is null)) return template;

            Guard.MustBeNotNull(item, nameof(item));
            Guard.MustBeOf<UserGuideSection>(item, nameof(item));

            throw new ArgumentException("Missing template");
        }
    }
}
