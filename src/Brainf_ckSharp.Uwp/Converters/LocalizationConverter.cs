using System.Diagnostics.Contracts;
using Microsoft.Toolkit.Uwp;

namespace Brainf_ckSharp.Uwp.Converters;

/// <summary>
/// A <see langword="class"/> with a collection of helper functions for bindings to localized resources
/// </summary>
public static class LocalizationConverter
{
    /// <summary>
    /// Returns a localized and formatted <see cref="string"/> value
    /// </summary>
    /// <param name="resource">The resource key to use</param>
    /// <param name="value">The value to format</param>
    /// <returns>The requested formatted and localized <see cref="string"/></returns>
    [Pure]
    public static string Convert(string resource, object value)
    {
        return string.Format(resource.GetLocalized(), value);
    }
}
