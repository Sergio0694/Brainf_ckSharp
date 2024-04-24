using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Brainf_ckSharp.Shared.Enums.Settings;
using CommunityToolkit.HighPerformance;

namespace Brainf_ckSharp.Uwp.Converters;

/// <summary>
/// A <see langword="class"/> with a collection of helper functions for bindings with a <see cref="Windows.UI.Xaml.Controls.Pivot"/> control
/// </summary>
public sealed class PivotSelectionConverter : IValueConverter
{
    /// <summary>
    /// Converts a given <see cref="ViewType"/> value into its corresponding index
    /// </summary>
    /// <param name="viewType">The input <see cref="ViewType"/> value</param>
    /// <returns>The <see cref="int"/> value representing <paramref name="viewType"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToIndex(ViewType viewType)
    {
        return (int)viewType;
    }

    /// <summary>
    /// Returns a <see cref="Visibility"/> value if the two arguments match
    /// </summary>
    /// <param name="viewType">The input <see cref="ViewType"/> value</param>
    /// <param name="target">The target <see cref="ViewType"/> value</param>
    /// <returns><see cref="Visibility.Visible"/> if the input values match, <see cref="Visibility.Collapsed"/> otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Visibility ConvertToVisibility(ViewType viewType, ViewType target)
    {
        return (Visibility)(viewType != target).ToByte();
    }

    /// <summary>
    /// Checks whether the input index matches a target value
    /// </summary>
    /// <param name="index">The input index to match</param>
    /// <param name="target">The target value to match</param>
    /// <returns><see langword="true"/> if the input values match, <see langword="false"/> otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ConvertIndexToBool(int index, int target)
    {
        return index == target;
    }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return ConvertToIndex((ViewType)value);
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return (ViewType)(int)value;
    }
}
