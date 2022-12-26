using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Uwp.Resources;

namespace Brainf_ckSharp.Uwp.Converters;

/// <summary>
/// A <see langword="class"/> with a collection of helper functions for bindings to <see cref="ushort"/> values
/// </summary>
public static class UInt16Converter
{
    /// <summary>
    /// Converts a given character into a <see cref="ushort"/> value
    /// </summary>
    /// <param name="c">The input character to convert</param>
    /// <returns>A <see cref="ushort"/> value for the input character</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Convert(char c)
    {
        return c;
    }

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
    public static Brush ConvertPositiveValueToAccentBrushOrFallback(ushort c)
    {
        return c > 0
            ? XamlResources.Brushes.SystemControlHighlightAccent
            : XamlResources.Brushes.ZeroValueInMemoryViewer;
    }
}
