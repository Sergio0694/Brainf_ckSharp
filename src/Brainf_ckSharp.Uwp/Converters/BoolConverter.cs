using System.Runtime.CompilerServices;

namespace Brainf_ckSharp.Uwp.Converters;

/// <summary>
/// A <see langword="class"/> that converts <see cref="bool"/> values
/// </summary>
public static class BoolConverter
{
    /// <summary>
    /// Checks whether any of the given inputs is <see langword="true"/>
    /// </summary>
    /// <param name="a">The first value to check</param>
    /// <param name="b">The second value to check</param>
    /// <param name="c">The third value to check</param>
    /// <returns>Whether or not any of the input values is <see langword="true"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any(bool a, bool b, bool c)
    {
        return a || b || c;
    }
}
