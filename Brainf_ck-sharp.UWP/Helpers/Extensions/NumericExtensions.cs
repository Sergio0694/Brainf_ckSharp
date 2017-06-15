using System.Runtime.CompilerServices;

namespace Brainf_ck_sharp_UWP.Helpers.Extensions
{
    /// <summary>
    /// A class with some extension methods for numeric types
    /// </summary>
    public static class NumericExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int value) => value >= 0 ? value : -value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(this double value) => value >= 0 ? value : -value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsWithDelta(this double value, double comparison, double delta = 0.1) => (value - comparison).Abs() < delta;
    }
}
