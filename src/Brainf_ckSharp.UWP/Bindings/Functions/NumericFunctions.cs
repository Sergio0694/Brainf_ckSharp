using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;

namespace Brainf_ckSharp.UWP.Bindings.Functions
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions for bindings to numeric values
    /// </summary>
    public static class NumericFunctions
    {
        /// <summary>
        /// Returns the input value if it's a positive number, or a fallback value otherwise
        /// </summary>
        /// <param name="value">The input value to check and possibly return</param>
        /// <param name="fallback">The fallback value to use if the input is not valid</param>
        /// <returns><paramref name="value"/> if it's a positive number, <paramref name="fallback"/> otherwise</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IntToPositiveValueOrFallback(int value, int fallback) => value >= 0 ? value : fallback;

        /// <summary>
        /// Converts a given character into a visible <see cref="string"/> representing its value in a visible way
        /// </summary>
        /// <param name="c">The input character to convert</param>
        /// <returns>A <see cref="string"/> with the original character, if visible, or an equivalent value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConvertToVisibleText(ushort c)
        {
            return c switch
            {
                32 => "SP",
                127 => "DEL",
                160 => "NBSP",
                173 => "SHY",
                _ when char.IsControl((char)c) => c.ToString(),
                _ => ((char)c).ToString()
            };
        }

        public static SolidColorBrush PositiveValueToAccentBrushOrFallback(ushort c)
        {
            return default;
        }
    }
}
