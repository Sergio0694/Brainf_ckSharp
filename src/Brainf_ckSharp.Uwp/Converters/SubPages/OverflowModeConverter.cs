using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="OverflowMode"/> values
    /// </summary>
    public static class OverflowModeConverter
    {
        /// <summary>
        /// Converts a <see cref="OverflowMode"/> value into its representation
        /// </summary>
        /// <param name="mode">The input <see cref="OverflowMode"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="OverflowMode"/> value</returns>
        [Pure]
        public static string Convert(OverflowMode mode)
        {
            return mode switch
            {
                OverflowMode.ByteWithOverflow => "Byte [0, 255]",
                OverflowMode.ByteWithNoOverflow => "Byte, no overflow",
                OverflowMode.UshortWithOverflow => "Signed short [0, 65535]",
                OverflowMode.UshortWithNoOverflow => "Signed short, no overflow",
                _ => throw new ArgumentException($"Invalid input value: {mode}", nameof(mode))
            };
        }
    }
}
