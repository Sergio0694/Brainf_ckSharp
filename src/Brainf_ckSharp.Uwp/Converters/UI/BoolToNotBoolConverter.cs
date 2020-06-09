using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

#nullable enable

namespace Brainf_ckSharp.Uwp.Converters.UI
{
    /// <summary>
    /// A <see langword="class"/> that negates <see cref="bool"/> values
    /// </summary>
    public static class BoolToNotBoolConverter
    {
        /// <summary>
        /// Inverts a given <see cref="bool"/> value
        /// </summary>
        /// <param name="value">The input <see cref="bool"/> value</param>
        /// <returns>The negation of <paramref name="value"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Convert(bool value) => !value;
    }
}
