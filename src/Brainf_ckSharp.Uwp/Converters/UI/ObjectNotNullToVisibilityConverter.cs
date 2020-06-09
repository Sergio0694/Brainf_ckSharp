using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Microsoft.Toolkit.HighPerformance.Extensions;

#nullable enable

namespace Brainf_ckSharp.Uwp.Converters.UI
{
    /// <summary>
    /// A <see langword="class"/> that converts <see cref="object"/> instances to <see cref="Visibility"/> values
    /// </summary>
    public static class ObjectNotNullToVisibilityConverter
    {
        /// <summary>
        /// Checks whether the input <see cref="object"/> is <see langword="null"/>
        /// </summary>
        /// <param name="obj">The input <see cref="object"/> instance</param>
        /// <returns><see cref="Visibility.Visible"/> if <paramref name="obj"/> is <see langword="null"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Visibility Convert(object? obj)
        {
            return (Visibility)(obj is null).ToInt();
        }
    }
}
