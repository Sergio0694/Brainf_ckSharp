using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace Brainf_ckSharp.Uwp.Extensions.System
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="bool"/> type
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Converts a <see cref="bool"/> value to the equivalent <see cref="Visibility"/> value
        /// </summary>
        /// <param name="flag">The input <see cref="bool"/> value to convert</param>
        /// <returns><see cref="Visibility.Visible"/> if <paramref name="flag"/> is <see langword="true"/>, <see cref="Visibility.Collapsed"/> otherwise</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Visibility ToVisibility(this bool flag)
        {
            int value = Unsafe.As<bool, byte>(ref flag);
            return (Visibility)value;
        }
    }
}
