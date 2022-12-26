using Brainf_ckSharp.Uwp.Resources;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;

#nullable enable

namespace Brainf_ckSharp.Uwp.Converters.Ide;

/// <summary>
/// A <see langword="class"/> that converts <see cref="bool"/> values to <see cref="Brush"/> values
/// </summary>
public static class BoolToForegroundBrushConverter
{
    /// <summary>
    /// Checks whether the input <see cref="bool"/> is <see langword="true"/>
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> value</param>
    /// <returns>A <see cref="Brush"/> corresponding to <paramref name="value"/>value</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Brush Convert(bool value)
    {
        return value switch
        {
            true => XamlResources.Brushes.MemoryCellSelectedForeground,
            false => XamlResources.Brushes.MemoryCellDefaultForeground
        };
    }
}
