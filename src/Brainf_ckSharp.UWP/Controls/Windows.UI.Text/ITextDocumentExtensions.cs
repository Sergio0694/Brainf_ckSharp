using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

#nullable enable

namespace Windows.UI.Text
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="ITextDocument"/> type
    /// </summary>
    public static class ITextDocumentExtensions
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
