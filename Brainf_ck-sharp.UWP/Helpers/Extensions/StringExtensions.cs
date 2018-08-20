using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.Extensions
{
    /// <summary>
    /// An extension class with some helper methods to work with strings
    /// </summary>
    public static class stringExtensions
    {
        /// <summary>
        /// Finds the coordinates in a multiline string for the given index
        /// </summary>
        /// <param name="text">The input text</param>
        /// <param name="index">The target text index</param>
        /// <param name="newline">The newline character to use</param>
        /// <remarks>The returned indexes are 1-based</remarks>
        [Pure]
        public static Coordinate FindCoordinates([NotNull] this string text, int index, char newline = '\r')
        {
            int
                row = 1,
                col = 1;
            for (int i = 0; i < index; i++)
            {
                if (text[i] == newline)
                {
                    row++;
                    col = 1;
                }
                else col++;
            }
            return new Coordinate(col, row);
        }

        /// <summary>
        /// Finds the indexes of the first character of each line indicated in the input
        /// </summary>
        /// <param name="text">The text to analyze</param>
        /// <param name="lines">The list of lines to look for</param>
        /// <param name="newline">The newline character to use</param>
        /// <remarks>The input lines list must have its elements 1-based</remarks>
        [Pure, NotNull]
        public static IReadOnlyList<int> FindLineIndexes([NotNull] this string text, [NotNull] IReadOnlyCollection<int> lines, char newline = '\r')
        {
            List<int> indexes = new List<int>();
            int line = 0;
            for (int i = 0; i < text.Length; i++)
                if (text[i] == newline && lines.Contains(++line))
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
        public static string GetLine([NotNull] this string text, int y, char newline = '\r')
        {
            string[] lines = text.Split(newline);
            int offset = y - 1;
            if (offset < 0 || offset > lines.Length - 1) throw new ArgumentOutOfRangeException(nameof(offset), "The line number is invalid");
            return lines[offset];
        }

        /// <summary>
        /// Aggregates a series of values into a <see cref="string"/> object in an efficient way
        /// </summary>
        /// <typeparam name="T">The type of the input values</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="converter">A function that converts each value to a text representation</param>
        [Pure, NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Aggregate<T>([NotNull] this IEnumerable<T> source, Func<T, string> converter)
        {
            return source.Aggregate(new StringBuilder(), (b, s) =>
            {
                b.Append(converter(s));
                return b;
            }).ToString();
        }

        /// <summary>
        /// Measures the rendering size of a text value
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <param name="size">The font size to use for the measurement</param>
        /// <param name="font">The font of the <see cref="TextBlock"/> used to measure the text</param>
        [Pure]
        public static Size MeasureText([NotNull] this string text, int size, [NotNull] FontFamily font)
        {
            TextBlock block = new TextBlock
            {
                FontSize = size,
                Text = text,
                FontFamily = font
            };
            block.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return block.DesiredSize;
        }

        /// <summary>
        /// Repeats a given character a set number of times
        /// </summary>
        /// <param name="c">The character to repeat</param>
        /// <param name="times">The repetitions in the resulting <see cref="string"/></param>
        [Pure]
        public static string Repeat(this char c, int times)
        {
            StringBuilder builder = new StringBuilder();
            while (times-- > 0) builder.Append(c);
            return builder.ToString();
        }

        /// <summary>
        /// Trimes the input <see langword="string"/> and adds the trailing ... if necessary
        /// </summary>
        /// <param name="text">The text to trim</param>
        /// <param name="length">The maximum length of the returned string (considering the ... too)</param>
        [Pure, NotNull]
        public static string Trim([NotNull] this string text, int length)
        {
            if (length < 4) throw new ArgumentOutOfRangeException(nameof(length), "The length must be at least 4");
            return text.Length <= length ? text : $"{text.Substring(0, length)}...";
        }
    }
}
