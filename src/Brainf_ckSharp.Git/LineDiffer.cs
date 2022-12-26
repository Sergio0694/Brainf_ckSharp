using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Git.Buffers;
using Brainf_ckSharp.Git.Enums;
using Brainf_ckSharp.Git.Models;
using Microsoft.Collections.Extensions;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.HighPerformance.Helpers;

namespace Brainf_ckSharp.Git;

/// <summary>
/// A <see langword="class"/> that implements the Paul Heckel git diff algorithm for line diffs
/// </summary>
public static class LineDiffer
{
    /// <summary>
    /// The maximum length of the input text segments to try the fast sequence comparison first
    /// </summary>
    private const int ShortPathNumberOfLinesThreshold = 2;

    /// <summary>
    /// The reusable <see cref="DictionarySlim{TKey,TValue}"/>
    /// </summary>
    private static readonly DictionarySlim<int, DiffEntry> LinesMap = new();

    /// <summary>
    /// Computes the line difference for a reference text and a new text
    /// </summary>
    /// <param name="oldText">The reference text to compare to</param>
    /// <param name="newText">The updated text to compare</param>
    /// <param name="separator">The separator character to use to split lines in <paramref name="oldText"/> and <paramref name="newText"/></param>
    /// <returns>A <see cref="MemoryOwner{T}"/> instance with the sequence of line modifications</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MemoryOwner<LineModificationType> ComputeDiff(string oldText, string newText, char separator)
    {
        return ComputeDiff(oldText.AsSpan(), newText.AsSpan(), separator);
    }

    /// <summary>
    /// Computes the line difference for a reference text and a new text
    /// </summary>
    /// <param name="oldText">The reference text to compare to</param>
    /// <param name="newText">The updated text to compare</param>
    /// <param name="separator">The separator character to use to split lines in <paramref name="oldText"/> and <paramref name="newText"/></param>
    /// <returns>A <see cref="MemoryOwner{T}"/> instance with the sequence of line modifications</returns>
    [Pure]
    public static MemoryOwner<LineModificationType> ComputeDiff(ReadOnlySpan<char> oldText, ReadOnlySpan<char> newText, char separator)
    {
        // If the new text is empty, no modifications are returned
        if (newText.IsEmpty)
        {
            return MemoryOwner<LineModificationType>.Empty;
        }

        int oldNumberOfLines = oldText.Count(separator) + 1;
        int newNumberOfLines = newText.Count(separator) + 1;

        // Fast path if the input text segments have the same length and are short enough
        if (oldText.Length == newText.Length &&
            oldNumberOfLines == newNumberOfLines &&
            oldNumberOfLines <= ShortPathNumberOfLinesThreshold &&
            oldText.SequenceEqual(newText))
        {
            return MemoryOwner<LineModificationType>.Allocate(newNumberOfLines, AllocationMode.Clear);
        }

        using SpanOwner<object?> oldTemporaryValues = SpanOwner<object?>.Allocate(oldNumberOfLines);
        using SpanOwner<object?> newTemporaryValues = SpanOwner<object?>.Allocate(newNumberOfLines);

        ref object? oldTemporaryValuesRef = ref oldTemporaryValues.DangerousGetReference();
        ref object? newTemporaryValuesRef = ref newTemporaryValues.DangerousGetReference();

        DictionarySlim<int, DiffEntry> table = LinesMap;
        table.Clear();

        Pool<DiffEntry>.Shared.Reset();

        // ==============
        // First pass
        // ==============
        // Iterate over all the lines in the new text file.
        // For each line, create the table entry if not present,
        // otherwise increment the line counter. Also set the
        // values in the temporary arrays to the table entires.
        int i = 0;
        foreach (ReadOnlySpan<char> line in newText.Tokenize(separator))
        {
            int hash = HashCode<char>.Combine(line);
            ref DiffEntry entry = ref table.GetOrAddValueRef(hash);

            if (entry is null)
            {
                entry = Pool<DiffEntry>.Shared.Rent();
                entry.NumberOfOccurrencesInNewText = 1;
                entry.NumberOfOccurrencesInOldText = 0;
                entry.LineNumberInOldText = 0;
            }
            else
            {
                if (entry.NumberOfOccurrencesInNewText == 1) entry.NumberOfOccurrencesInNewText = 2;
                else entry.NumberOfOccurrencesInNewText = int.MaxValue;
            }

            Unsafe.Add(ref newTemporaryValuesRef, i) = entry;
            i += 1;
        }

        // ==============
        // Second pass
        // ==============
        // Same as the first pass, but acting on the old text,
        // and the associated temporary values and table entry fields.
        int j = 0;
        foreach (ReadOnlySpan<char> line in oldText.Tokenize(separator))
        {
            int hash = HashCode<char>.Combine(line);
            ref DiffEntry entry = ref table.GetOrAddValueRef(hash);

            if (entry is null)
            {
                entry = Pool<DiffEntry>.Shared.Rent();
                entry.NumberOfOccurrencesInNewText = 0;
                entry.NumberOfOccurrencesInOldText = 1;
                entry.LineNumberInOldText = 0;
            }
            else
            {
                if (entry.NumberOfOccurrencesInOldText < 2) entry.NumberOfOccurrencesInOldText++;
                else entry.NumberOfOccurrencesInOldText = int.MaxValue;
            }

            entry.LineNumberInOldText = j;
            Unsafe.Add(ref oldTemporaryValuesRef, j) = entry;
            j += 1;
        }

        // ==============
        // Third pass
        // ==============
        // If a line exactly only once in both files, it means it's the same
        // line, although it might have been moved to a different location.
        // These are the only affected lines in this pass.
        i = 0;
        for (; i < newNumberOfLines; i++)
        {
            if (Unsafe.Add(ref newTemporaryValuesRef, i) is DiffEntry entry &&
                entry.NumberOfOccurrencesInOldText == 1 &&
                entry.NumberOfOccurrencesInNewText == 1)
            {
                int olno = entry.LineNumberInOldText;
                Unsafe.Add(ref newTemporaryValuesRef, i) = olno;
                Unsafe.Add(ref oldTemporaryValuesRef, olno) = i;
            }
        }

        // ==============
        // Fourth pass
        // ==============
        // If a line doesn't have any changes, and the lines immediately
        // adjacent to it in both files are identical, this means the
        // line is the same line as well. This can be used to find
        // blocks of unchanged lines across the two text versions.
        for (i = 0; i < newNumberOfLines - 1; i++)
        {
            if (Unsafe.Add(ref newTemporaryValuesRef, i) is int k &&
                k + 1 < oldNumberOfLines)
            {
                ref object? newTemporaryValue = ref Unsafe.Add(ref newTemporaryValuesRef, i + 1);
                ref object? oldTemporaryValue = ref Unsafe.Add(ref oldTemporaryValuesRef, k + 1);

                if (newTemporaryValue!.Equals(oldTemporaryValue))
                {
                    newTemporaryValue = k + 1;
                    oldTemporaryValue = i + 1;
                }
            }
        }

        // ==============
        // Fifth pass
        // ==============
        // Sames as the previous step, but acting in descending order.
        for (i = newNumberOfLines - 1; i > 0; i--)
        {
            if (Unsafe.Add(ref newTemporaryValuesRef, i) is int k &&
                k - 1 >= 0)
            {
                ref object? newTemporaryValue = ref Unsafe.Add(ref newTemporaryValuesRef, i - 1);
                ref object? oldTemporaryValue = ref Unsafe.Add(ref oldTemporaryValuesRef, k - 1);

                if (newTemporaryValue!.Equals(oldTemporaryValue))
                {
                    newTemporaryValue = k - 1;
                    oldTemporaryValue = i - 1;
                }
            }
        }

        // Allocate the result array with on entry per line in the updated text
        MemoryOwner<LineModificationType> result = MemoryOwner<LineModificationType>.Allocate(newNumberOfLines);

        ref LineModificationType resultRef = ref result.DangerousGetReference();

        // ==============
        // Final pass
        // ==============
        // Each entry in the result array is set by reading data from the
        // temporary values for the new text. If an entry is an int it
        // means that that line was present in the old file too and in the
        // same location. Otherwise, if a table entry is present,
        // it means that the current line has been modified in some way.
        for (i = 0; i < newNumberOfLines; i++)
        {
            Unsafe.Add(ref resultRef, i) = Unsafe.Add(ref newTemporaryValuesRef, i) switch
            {
                int _ => LineModificationType.None,
                _ => LineModificationType.Modified
            };
        }

        return result;
    }
}
