using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Uwp.Resources;

namespace Brainf_ckSharp.Uwp.Bindings.Functions
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions for bindings to numeric values
    /// </summary>
    public static class NumericFunctions
    {
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

        /// <summary>
        /// Gets either the accent brush or a fallback brush depending on the input value
        /// </summary>
        /// <param name="c">The input value to check</param>
        /// <returns>The accent brush if <paramref name="c"/> is positive, a fallback brush otherwise</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Brush PositiveValueToAccentBrushOrFallback(ushort c)
        {
            return c > 0 ? XamlResources.AccentBrush : XamlResources.Get<Brush>("ZeroValueInMemoryViewerBrush");
        }
    }
}
