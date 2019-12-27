using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

#nullable enable

namespace Windows.UI.Text
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="ITextDocument"/> type
    /// </summary>
    internal static class ITextDocumentExtensions
    {
        /// <summary>
        /// Gets the plain text from the input <see cref="ITextDocument"/> instance
        /// </summary>
        /// <param name="document">The document to read the text from</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetText(this ITextDocument document)
        {
            document.GetText(TextGetOptions.None, out string text);
            return text;
        }

        /// <summary>
        /// Sets the foreground color of a given range in the input <see cref="ITextDocument"/> instance
        /// </summary>
        /// <param name="document">The input <see cref="ITextDocument"/> instance to modify</param>
        /// <param name="start">The start index of the range to modify</param>
        /// <param name="end">The end index of the range to modify</param>
        /// <param name="color">The color to use for the target text range</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRangeColor(this ITextDocument document, int start, int end, Color color)
        {
            ITextRange range = document.GetRange(start, end);
            range.CharacterFormat.ForegroundColor = color;
        }

        /// <summary>
        /// Gets the plain text from the input <see cref="ITextRange"/> instance
        /// </summary>
        /// <param name="range">The range to read the text from</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetText(this ITextRange range)
        {
            range.GetText(TextGetOptions.None, out string text);
            return text;
        }
    }
}