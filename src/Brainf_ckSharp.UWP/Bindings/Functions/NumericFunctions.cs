using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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
    }
}
