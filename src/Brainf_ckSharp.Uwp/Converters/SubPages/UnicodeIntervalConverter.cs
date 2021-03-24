using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Models;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Uwp;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="UnicodeInterval"/> instances
    /// </summary>
    public static class UnicodeIntervalConverter
    {
        // Localized resources
        private static readonly string ControlCharacters = "UnicodeInterval/ControlCharacters".GetLocalized();
        private static readonly string NonVisible = "UnicodeInterval/NonVisible".GetLocalized();

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
                (0, 31) => ControlCharacters,
                (128, 159) => NonVisible,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(nameof(interval), "Invalid unicode interval")
            };
        }
    }
}
