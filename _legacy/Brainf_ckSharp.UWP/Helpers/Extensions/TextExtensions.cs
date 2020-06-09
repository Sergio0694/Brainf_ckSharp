using Windows.UI.Text;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions
{
    /// <summary>
    /// A class with some extensions to deal with text documents
    /// </summary>
    public static class TextExtensions
    {
        /// <summary>
        /// Returns the start and end positions for an <see cref="ITextRange"/> instance, so that start >= end no matter the selection direction
        /// </summary>
        /// <param name="range">The input range to analyze</param>
        public static (int Start, int End) GetAbsPositions([NotNull] this ITextRange range)
        {
            if (range.StartPosition > range.EndPosition) return (range.EndPosition, range.StartPosition);
            return (range.StartPosition, range.EndPosition);
        }
    }
}
