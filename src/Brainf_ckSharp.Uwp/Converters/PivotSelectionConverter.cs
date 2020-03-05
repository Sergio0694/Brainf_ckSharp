using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace Brainf_ckSharp.Uwp.Converters
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions for bindings with a <see cref="Windows.UI.Xaml.Controls.Pivot"/> control
    /// </summary>
    public static class PivotSelectionConverter
    {
        /// <summary>
        /// Checks whether the input index matches a target value
        /// </summary>
        /// <param name="index">The input index to match</param>
        /// <param name="target">The target value to match</param>
        /// <returns><see langword="true"/> if the input values match, <see langword="false"/> otherwise</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ConvertIndexToBool(int index, int target) => index == target;

        /// <summary>
        /// Checks whether the input index matches a target value
        /// </summary>
        /// <param name="index">The input index to match</param>
        /// <param name="target">The target value to match</param>
        /// <returns><see cref="Visibility.Visible"/> if the input values match, <see cref="Visibility.Collapsed"/> otherwise</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Visibility ConvertIndexToVisibility(int index, int target) => (Visibility)(index != target).ToInt();
    }
}
