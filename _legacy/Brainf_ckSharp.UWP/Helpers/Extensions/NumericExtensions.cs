using System;
using System.Runtime.CompilerServices;

namespace Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions
{
    /// <summary>
    /// A class with some extension methods for numeric types
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Returns the module of the given integer value
        /// </summary>
        /// <param name="value">The input value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int value) => value >= 0 ? value : -value;

        /// <summary>
        /// Returns the module of the given double value
        /// </summary>
        /// <param name="value">The input value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(this double value) => value >= 0 ? value : -value;

        /// <summary>
        /// Compares the double values with a given threshold
        /// </summary>
        /// <param name="value">The first value</param>
        /// <param name="comparison">The comparison value</param>
        /// <param name="delta">The comparison threshold (tolerance), the default is 0.1</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsWithDelta(this double value, double comparison, double delta = 0.1) => (value - comparison).Abs() < delta;

        /// <summary>
        /// Converts a character code to its Segoe MDL2 Assets string
        /// </summary>
        /// <param name="value">The value to convert</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToSegoeMDL2Icon(this int value) => Convert.ToChar(value).ToString();
    }
}
