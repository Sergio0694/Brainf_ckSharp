using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Text;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide.Extensions.Windows.UI.Text;

/// <summary>
/// A <see langword="class"/> with some extension methods for the <see cref="ITextRange"/> type
/// </summary>
internal static class ITextRaangeExtensions
{
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

    /// <summary>
    /// Returns the start and end positions for an <see cref="ITextRange"/> instance, so that start &lt;= end no matter the selection direction
    /// </summary>
    /// <param name="range">The input range to analyze</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Start, int End) GetBounds(this ITextRange range)
    {
        int start = range.StartPosition, end = range.EndPosition;

        return start <= end ? (start, end) : (end, start);
    }
}