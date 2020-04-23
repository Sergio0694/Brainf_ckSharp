using System.Diagnostics.Contracts;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages.Settings
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to convert <see cref="BracketsFormattingStyle"/> values
    /// </summary>
    public static class BracketsFormattingStyleConverter
    {
        /// <summary>
        /// Converts a <see cref="BracketsFormattingStyle"/> value into its representation
        /// </summary>
        /// <param name="value">The input <see cref="BracketsFormattingStyle"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="BracketsFormattingStyle"/> value</returns>
        [Pure]
        public static BracketsFormattingStyle Convert(bool value)
        {
            return value ? BracketsFormattingStyle.NewLine : BracketsFormattingStyle.SameLine;
        }
    }
}
