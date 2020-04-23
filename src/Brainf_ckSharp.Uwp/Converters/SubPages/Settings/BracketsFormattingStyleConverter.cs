using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages.Settings
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to convert <see cref="BracketsFormattingStyle"/> values
    /// </summary>
    public static class BracketsFormattingStyleConverter
    {
        /// <summary>
        /// Converts a <see cref="CodeLibrarySection"/> instance into its representation
        /// </summary>
        /// <param name="section">The input <see cref="CodeLibrarySection"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="CodeLibrarySection"/> instance</returns>
        [Pure]
        public static BracketsFormattingStyle Convert(bool value)
        {
            return value ? BracketsFormattingStyle.NewLine : BracketsFormattingStyle.SameLine;
        }
    }
}
