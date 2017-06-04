using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Helpers
{
    /// <summary>
    /// An internal helper class with some extension methods
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Reconstructs a <see cref="String"/> from a sequence of characters
        /// </summary>
        /// <param name="source">The source list of characters to concatenate</param>
        [Pure, NotNull]
        public static String AggregateToString([NotNull] this IEnumerable<char> source)
        {
            return source.Aggregate(new StringBuilder(), (b, c) =>
            {
                b.Append(c);
                return b;
            }).ToString();
        }
    }
}
