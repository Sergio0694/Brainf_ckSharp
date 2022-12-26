using System.Runtime.CompilerServices;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide.Extensions.Windows.System;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDown(this VirtualKey key)
    {
        CoreVirtualKeyStates state = Window.Current.CoreWindow.GetKeyState(key);

        return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
    }
}
