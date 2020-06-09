using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="UserGuideSection"/> values
    /// </summary>
    public static class UserGuideSectionConverter
    {
        /// <summary>
        /// Converts a <see cref="UserGuideSection"/> value into its title representation
        /// </summary>
        /// <param name="section">The input <see cref="UserGuideSection"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="UserGuideSection"/> value</returns>
        [Pure]
        public static string ConvertTitle(UserGuideSection section)
        {
            return section switch
            {
                UserGuideSection.Introduction => "Introduction",
                UserGuideSection.PBrain => "PBrain",
                UserGuideSection.Debugging => "Debugging",
                UserGuideSection.Samples => "Samples",
                _ => throw new ArgumentException($"Invalid input value: {section}", nameof(section))
            };
        }

        /// <summary>
        /// Converts a <see cref="UserGuideSection"/> value into its description representation
        /// </summary>
        /// <param name="section">The input <see cref="UserGuideSection"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="UserGuideSection"/> value</returns>
        [Pure]
        public static string ConvertDescription(UserGuideSection section)
        {
            return section switch
            {
                UserGuideSection.Introduction => "Learn how to use the Brainf*ck language",
                UserGuideSection.PBrain => "Do more by using the PBrain extension operators",
                UserGuideSection.Debugging => "Find errors more easily with debugging features",
                UserGuideSection.Samples => "Some code samples to learn the basics",
                _ => throw new ArgumentException($"Invalid input value: {section}", nameof(section))
            };
        }
    }
}
