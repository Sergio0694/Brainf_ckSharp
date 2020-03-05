using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Uwp.Models;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="UnicodeInterval"/> instances
    /// </summary>
    public static class UnicodeIntervalConverter
    {
        /// <summary>
        /// Converts a <see cref="UnicodeInterval"/> instance into its description representation
        /// </summary>
        /// <param name="interval">The input <see cref="UnicodeInterval"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="UnicodeInterval"/> instance</returns>
        [Pure]
        public static string ConvertDescription(UnicodeInterval interval)
        {
            return (interval.Start, interval.End) switch
            {
                (0, 31) => "Control characters",
                (128, 159) => "Non visible",
                _ => throw new ArgumentOutOfRangeException(nameof(interval), $"Invalid unicode interval: {interval}")
            };
        }
    }
}
