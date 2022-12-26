using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Brainf_ckSharp.Services;

#nullable enable

namespace Brainf_ckSharp.Uwp.Services.Keyboard;

/// <summary>
/// A <see langword="class"/> that listens to the global <see cref="UIElement.CharacterReceived"/> event for the current app window
/// </summary>
public sealed class KeyboardListenerService : IKeyboardListenerService
{
    /// <summary>
    /// Lock target for <see cref="IKeyboardListenerService.CharacterReceived"/>
    /// </summary>
    private readonly object Lock = new();

    /// <summary>
    /// Private backing event for <see cref="IKeyboardListenerService.CharacterReceived"/>
    /// </summary>
    private event Action<char>? CharacterReceived;

    /// <inheritdoc/>
    event Action<char> IKeyboardListenerService.CharacterReceived
    {
        add
        {
            lock (Lock)
            {
                if (CharacterReceived is null)
                {
                    Window.Current.Content.CharacterReceived += Content_CharacterReceived;
                }

                CharacterReceived += value;
            }
        }
        remove
        {
            lock (Lock)
            {
                CharacterReceived -= value;

                if (CharacterReceived is null)
                {
                    Window.Current.Content.CharacterReceived -= Content_CharacterReceived;
                }
            }
        }
    }

    /// <summary>
    /// Handles a single character being entered by the user
    /// </summary>
    /// <param name="sender">The sender <see cref="UIElement"/> for the current invocation</param>
    /// <param name="args">The <see cref="CharacterReceivedRoutedEventArgs"/> instance with the pressed character</param>
    private void Content_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
    {
        CharacterReceived?.Invoke(args.Character);
    }
}
