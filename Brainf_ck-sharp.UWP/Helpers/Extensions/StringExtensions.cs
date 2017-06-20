using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.Extensions
{
    /// <summary>
    /// An extension class with some helper methods to work with strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Finds the coordinates in a multiline string for the given index
        /// </summary>
        /// <param name="text">The input text</param>
        /// <param name="index">The target text index</param>
        /// <param name="newline">The newline character to use</param>
        /// <remarks>The returned indexes are 1-based</remarks>
        [Pure]
        public static (int Y, int X) FindCoordinates([NotNull] this String text, int index, char newline = '\r')
        {
            int
                row = 1,
                col = 1;
            for (int i = 0; i < index; i++)
            {
                if (text[i] == '\r')
                {
                    row++;
                    col = 1;
                }
                else col++;
            }
            return (row, col);
        }

        /// <summary>
        /// Finds the indexes of the first character of each line indicated in the input
        /// </summary>
        /// <param name="text">The text to analyze</param>
        /// <param name="lines">The list of lines to look for</param>
        /// <param name="newline">The newline character to use</param>
        /// <remarks>The input lines list must have its elements 1-based</remarks>
        [Pure, NotNull]
        public static IReadOnlyList<int> FindIndexes([NotNull] this String text, [NotNull] IReadOnlyCollection<int> lines, char newline = '\r')
        {
            List<int> indexes = new List<int>();
            int line = 0;
            for (int i = 0; i < text.Length; i++)
                if (text[i] == '\r' && lines.Contains(++line))
                    indexes.Add(i);
            return indexes;
        }

        /// <summary>
        /// Returns a specific line from the input text
        /// </summary>
        /// <param name="text">The input text to split into lines</param>
        /// <param name="y">The target line number (1-based)</param>
        /// <param name="newline">The default newline character used to extract the lines</param>
        [Pure, NotNull]
        public static String GetLine([NotNull] this String text, int y, char newline = '\r')
        {
            String[] lines = text.Split(newline);
            int offset = y - 1;
            if (offset < 0 || offset > lines.Length - 1) throw new ArgumentOutOfRangeException("The line number is invalid");
            return lines[offset];
        }
    }
}
