using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Git.Enums;
using Brainf_ckSharp.Git.Models;
using Microsoft.Collections.Extensions;

namespace Brainf_ckSharp.Git
{
    /// <summary>
    /// A <see langword="class"/> that implements the Paul Heckel git diff algorithm for line diffs
    /// </summary>
    public static class LineDiffer
    {
        /// <summary>
        /// The reusable <see cref="DictionarySlim{TKey,TValue}"/>
        /// </summary>
        private static readonly DictionarySlim<int, DiffEntry> LinesMap = new DictionarySlim<int, DiffEntry>();

        /// <summary>
        /// Computes the line difference for a reference text and a new text
        /// </summary>
        /// <param name="oldText">The reference text to compare to</param>
        /// <param name="newText">The updated text to compare</param>
        /// <returns>A <see cref="MemoryOwner{T}"/> instance with the sequence of line modifications</returns>
        [Pure]
        public static MemoryOwner<LineModificationType> ComputeDiff(string oldText, string newText, char separator)
        {
            int OL = oldText.Count(separator) + 1;
            int NL = newText.Count(separator) + 1;

            object[] OA = ArrayPool<object>.Shared.Rent(OL);
            object[] NA = ArrayPool<object>.Shared.Rent(NL);

            DictionarySlim<int, DiffEntry> table = LinesMap;
            table.Clear();

            Pool<DiffEntry>.Reset();

            try
            {
                // First pass
                int i = 0;
                foreach (ReadOnlySpan<char> line in newText.Tokenize(separator))
                {
                    int hash = line.GetxxHash32Code();
                    ref DiffEntry entry = ref table.GetOrAddValueRef(hash);

                    if (entry is null)
                    {
                        entry = Pool<DiffEntry>.Rent();
                        entry.NumberOfOccurrencesInNewText = 1;
                        entry.NumberOfOccurrencesInOldText = 0;
                        entry.LineNumberInOldText = 0;
                    }
                    else
                    {
                        if (entry.NumberOfOccurrencesInNewText == 1) entry.NumberOfOccurrencesInNewText = 2;
                        else entry.NumberOfOccurrencesInNewText = int.MaxValue;
                    }

                    NA[i] = entry;
                    i += 1;
                }

                // Second pass
                int j = 0;
                foreach (ReadOnlySpan<char> line in oldText.Tokenize(separator))
                {
                    int hash = line.GetxxHash32Code();
                    ref DiffEntry entry = ref table.GetOrAddValueRef(hash);

                    if (entry is null)
                    {
                        entry = Pool<DiffEntry>.Rent();
                        entry.NumberOfOccurrencesInNewText = 0;
                        entry.NumberOfOccurrencesInOldText = 1;
                        entry.LineNumberInOldText = 0;
                    }
                    else
                    {
                        if (entry.NumberOfOccurrencesInOldText == 0) entry.NumberOfOccurrencesInOldText = 1;
                        else if (entry.NumberOfOccurrencesInOldText == 1) entry.NumberOfOccurrencesInOldText = 2;
                        else entry.NumberOfOccurrencesInOldText = int.MaxValue;
                    }

                    entry.LineNumberInOldText = j;
                    OA[j] = entry;
                    j += 1;
                }

                // Third pass
                i = 0;
                for (; i < NL; i++)
                {
                    if (NA[i] is DiffEntry entry &&
                        entry.NumberOfOccurrencesInOldText == 1 &&
                        entry.NumberOfOccurrencesInNewText == 1)
                    {
                        int olno = entry.LineNumberInOldText;
                        NA[i] = olno;
                        OA[olno] = i;
                    }
                }

                // Fourth pass
                for (i = 0; i < NL - 1; i++)
                {
                    if (NA[i] is int k &&
                        k + 1 < OL &&
                        NA[i + 1].Equals(OA[k + 1]))
                    {
                        NA[i + 1] = k + 1;
                        OA[k + 1] = i + 1;
                    }
                }

                // Fifth pass
                for (i = NL - 1; i > 0; i--)
                {
                    if (NA[i] is int k &&
                        k - 1 >= 0 &&
                        NA[i - 1].Equals(OA[k - 1]))
                    {
                        NA[i - 1] = k - 1;
                        OA[k - 1] = i - 1;
                    }
                }

                MemoryOwner<LineModificationType> result = MemoryOwner<LineModificationType>.Allocate(NL);
                ref LineModificationType resultRef = ref result.GetReference();

                for (i = 0; i < NL; i++)
                {
                    if (NA[i] is int) Unsafe.Add(ref resultRef, i) = LineModificationType.None;
                    else Unsafe.Add(ref resultRef, i) = LineModificationType.Modified;
                }

                return result;
            }
            finally
            {
                ArrayPool<object>.Shared.Return(OA);
                ArrayPool<object>.Shared.Return(NA);
            }
        }
    }
}
