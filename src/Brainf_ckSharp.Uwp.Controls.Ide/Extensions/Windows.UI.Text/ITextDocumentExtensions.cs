﻿using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Text;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide.Extensions.Windows.UI.Text;

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
    /// Gets a text range from an <see cref="ITextDocument"/> instance at a specified position
    /// </summary>
    /// <param name="document">The input document</param>
    /// <param name="position">The position for the range to retrieve</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITextRange GetRangeAt(this ITextDocument document, int position)
    {
        return document.GetRange(position, position);
    }

    /// <summary>
    /// Sets the foreground color of a given range in the input <see cref="ITextDocument"/> instance
    /// </summary>
    /// <param name="document">The input <see cref="ITextDocument"/> instance to modify</param>
    /// <param name="start">The start index of the range to modify</param>
    /// <param name="end">The end index of the range to modify</param>
    /// <param name="color">The color to use for the target text range</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetRangeColor(this ITextDocument document, int start, int end, Color color)
    {
        ITextRange range = document.GetRange(start, end);

        range.CharacterFormat.ForegroundColor = color;
    }

    /// <summary>
    /// Sets the tab length for the input <see cref="ITextDocument"/> instance
    /// </summary>
    /// <param name="document">The input <see cref="ITextDocument"/> instance to modify</param>
    /// <param name="length">The tab length value to set</param>
    public static void SetTabLength(this ITextDocument document, int length)
    {
        document.DefaultTabStop = length * 3; // Each space has an approximate width of 3 points

        ITextParagraphFormat format = document.GetDefaultParagraphFormat();

        format.ClearAllTabs();

        document.SetDefaultParagraphFormat(format);
    }

    /// <summary>
    /// Clears the undo history for a given input <see cref="ITextDocument"/> instance
    /// </summary>
    /// <param name="document">The input <see cref="ITextDocument"/> instance to modify</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ClearUndoHistory(this ITextDocument document)
    {
        uint limit = document.UndoLimit;

        document.UndoLimit = 0;
        document.UndoLimit = limit;
    }

    /// <summary>
    /// Loads a plain text document into the target <see cref="ITextDocument"/> instance
    /// </summary>
    /// <param name="document">The document to use to load the text</param>
    /// <param name="text">The plain text to load</param>
    public static void LoadFromString(this ITextDocument document, string text)
    {
        string trimmed = text.Equals("\r") ? string.Empty : text;

        document.SetText(TextSetOptions.None, trimmed);

        document.ClearUndoHistory();
    }
}
