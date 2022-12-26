﻿using System.Runtime.CompilerServices;
using Windows.UI.Text;

#nullable enable

namespace Brainf_ckSharp.Uwp.Converters.UI;

/// <summary>
/// A <see langword="class"/> that converts <see cref="bool"/> values to <see cref="FontWeight"/> values
/// </summary>
public static class BoolToFontWeightConverter
{
    /// <summary>
    /// Checks whether the input <see cref="bool"/> is <see langword="true"/>
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> value</param>
    /// <returns><see cref="FontWeights.Bold"/> if <paramref name="value"/> is <see langword="true"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FontWeight Convert(bool value)
    {
        return value switch
        {
            true => FontWeights.Bold,
            false => FontWeights.Normal
        };
    }
}
