using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml;

#nullable enable

namespace Windows.System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="VirtualKey"/> type
    /// </summary>
    internal static class VirtualKeyExtensions
    {
        /// <summary>
        /// Gets whether or not a given key is currently pressed for the current window
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns><see langword="true"/> if <paramref name="key"/> is pressed, <see langword="false"/> otherwise</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDown(this VirtualKey key)
        {
            CoreVirtualKeyStates state = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}
