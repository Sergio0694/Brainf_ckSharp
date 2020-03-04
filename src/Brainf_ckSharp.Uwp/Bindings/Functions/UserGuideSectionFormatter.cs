using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Uwp.Enums;

namespace Brainf_ckSharp.Uwp.Bindings.Functions
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="UserGuideSection"/> instances
    /// </summary>
    public static class UserGuideSectionFormatter
    {
        /// <summary>
        /// Formats the title of a <see cref="UserGuideSection"/> instance
        /// </summary>
        /// <param name="section">The input <see cref="UserGuideSection"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="UserGuideSection"/> instance</returns>
        [Pure]
        public static string FormatTitle(UserGuideSection section)
        {
            if (section == UserGuideSection.Introduction) return "Introduction";
            if (section == UserGuideSection.PBrain) return "PBrain";
            if (section == UserGuideSection.Debugging) return "Debugging";
            if (section == UserGuideSection.Samples) return "Samples";

            throw new ArgumentException($"Invalid input value: {section}", nameof(section));
        }

        /// <summary>
        /// Formats the title of a <see cref="UserGuideSection"/> instance
        /// </summary>
        /// <param name="section">The input <see cref="UserGuideSection"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="UserGuideSection"/> instance</returns>
        [Pure]
        public static string FormatDescription(UserGuideSection section)
        {
            if (section == UserGuideSection.Introduction) return "Learn how to use the Brainf*ck language";
            if (section == UserGuideSection.PBrain) return "Do more by using the PBrain extension operators";
            if (section == UserGuideSection.Debugging) return "Find errors more easily with debugging features";
            if (section == UserGuideSection.Samples) return "Some code samples to learn the basics";

            throw new ArgumentException($"Invalid input value: {section}", nameof(section));
        }
    }
}
