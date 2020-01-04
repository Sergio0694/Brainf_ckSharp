using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Git.Enums;
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
        private static readonly DictionarySlim<int, Entry> LinesMap = new DictionarySlim<int, Entry>();

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

            DictionarySlim<int, Entry> table = LinesMap;
            table.Clear();

            Pool<Entry>.Reset();

            try
            {
                // First pass
                int i = 0;
                foreach (ReadOnlySpan<char> line in newText.Tokenize(separator))
                {
                    int hash = line.GetxxHash32Code();
                    ref Entry entry = ref table.GetOrAddValueRef(hash);

                    if (entry is null)
                    {
                        entry = Pool<Entry>.Rent();
                        entry.NC = 1;
                        entry.OC = 0;
                        entry.OLNO = 0;
                    }
                    else
                    {
                        if (entry.NC == 1) entry.NC = 2;
                        else entry.NC = int.MaxValue;
                    }

                    NA[i] = entry;
                    i += 1;
                }

                // Second pass
                int j = 0;
                foreach (ReadOnlySpan<char> line in oldText.Tokenize(separator))
                {
                    int hash = line.GetxxHash32Code();
                    ref Entry entry = ref table.GetOrAddValueRef(hash);

                    if (entry is null)
                    {
                        entry = Pool<Entry>.Rent();
                        entry.NC = 0;
                        entry.OC = 1;
                        entry.OLNO = 0;
                    }
                    else
                    {
                        if (entry.OC == 0) entry.OC = 1;
                        else if (entry.OC == 1) entry.OC = 2;
                        else entry.OC = int.MaxValue;
                    }

                    entry.OLNO = j;
                    OA[j] = entry;
                    j += 1;
                }

                // Third pass
                i = 0;
                for (; i < NL; i++)
                {
                    if (NA[i] is Entry entry &&
                        entry.OC == 1 &&
                        entry.NC == 1)
                    {
                        int olno = entry.OLNO;
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

        public sealed class Entry
        {
            public int OC;
            public int NC;
            public int OLNO;

            public override string ToString()
            {
                return $"OC: {OC}, NC: {NC}, OLNO: {OLNO}";
            }
        }
    }
}
