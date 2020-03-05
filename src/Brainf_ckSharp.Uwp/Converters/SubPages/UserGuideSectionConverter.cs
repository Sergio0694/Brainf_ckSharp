using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Uwp.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="UserGuideSection"/> instances
    /// </summary>
    public static class UserGuideSectionConverter
    {
        /// <summary>
        /// Converts a <see cref="UserGuideSection"/> instance into its title representation
        /// </summary>
        /// <param name="section">The input <see cref="UserGuideSection"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="UserGuideSection"/> instance</returns>
        [Pure]
        public static string ConvertTitle(UserGuideSection section)
        {
            if (section == UserGuideSection.Introduction) return "Introduction";
            if (section == UserGuideSection.PBrain) return "PBrain";
            if (section == UserGuideSection.Debugging) return "Debugging";
            if (section == UserGuideSection.Samples) return "Samples";

            throw new ArgumentException($"Invalid input value: {section}", nameof(section));
        }

        /// <summary>
        /// Converts a <see cref="UserGuideSection"/> instance into its description representation
        /// </summary>
        /// <param name="section">The input <see cref="UserGuideSection"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="UserGuideSection"/> instance</returns>
        [Pure]
        public static string ConvertDescription(UserGuideSection section)
        {
            if (section == UserGuideSection.Introduction) return "Learn how to use the Brainf*ck language";
            if (section == UserGuideSection.PBrain) return "Do more by using the PBrain extension operators";
            if (section == UserGuideSection.Debugging) return "Find errors more easily with debugging features";
            if (section == UserGuideSection.Samples) return "Some code samples to learn the basics";

            throw new ArgumentException($"Invalid input value: {section}", nameof(section));
        }
    }
}
