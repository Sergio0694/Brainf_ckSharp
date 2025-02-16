using System;
using CommunityToolkit.WinUI;

namespace Brainf_ckSharp.Uwp.Converters;

/// <summary>
/// A <see langword="class"/> with a collection of helper functions for bindings to <see cref="Enum"/> values
/// </summary>
public static class EnumConverter
{
    /// <summary>
    /// Returns a localized version of the input <see cref="Enum"/> value
    /// </summary>
    /// <param name="value">The input value to localize</param>
    /// <returns>A localized representation for <paramref name="value"/></returns>
    [Obsolete("This method is needed to trick the XAML compiler, use the generic version instead")]
    public static string Convert(object value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a localized version of the input <see cref="Enum"/> value
    /// </summary>
    /// <typeparam name="T">The input<see cref="Enum"/> value to localize</typeparam>
    /// <param name="value">The input value to localize</param>
    /// <returns>A localized representation for <paramref name="value"/></returns>
    public static string Convert<T>(T value) where T : struct, Enum
    {
        return $"{typeof(T).Name}/{value}".GetLocalized()!;
    }

    /// <summary>
    /// Returns a localized version of the input <see cref="Enum"/> value
    /// </summary>
    /// <param name="value">The input value to localize</param>
    /// <param name="scope">The scope to use to retrieve the localized text</param>
    /// <returns>A localized representation for <paramref name="value"/></returns>
    public static string Convert(object value, string scope)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a localized version of the input <see cref="Enum"/> value
    /// </summary>
    /// <typeparam name="T">The input<see cref="Enum"/> value to localize</typeparam>
    /// <param name="value">The input value to localize</param>
    /// <param name="scope">The scope to use to retrieve the localized text</param>
    /// <returns>A localized representation for <paramref name="value"/></returns>
    public static string Convert<T>(T value, string scope) where T : struct, Enum
    {
        return $"{typeof(T).Name}/{value}/{scope}".GetLocalized()!;
    }
}
