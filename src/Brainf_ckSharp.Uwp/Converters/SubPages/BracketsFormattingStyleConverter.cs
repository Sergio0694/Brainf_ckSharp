using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums.Settings;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="BracketsFormattingStyle"/> values
    /// </summary>
    public static class BracketsFormattingStyleConverter
    {
        /// <summary>
        /// Converts a <see cref="BracketsFormattingStyle"/> value into its representation
        /// </summary>
        /// <param name="style">The input <see cref="BracketsFormattingStyle"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="BracketsFormattingStyle"/> value</returns>
        [Pure]
        public static string Convert(BracketsFormattingStyle style)
        {
            return style switch
            {
                BracketsFormattingStyle.NewLine => "New line",
                BracketsFormattingStyle.SameLine => "Same line",
                _ => throw new ArgumentException($"Invalid input value: {style}", nameof(style))
            };
        }
    }
}
