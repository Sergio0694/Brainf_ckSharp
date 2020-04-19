using System;

namespace Brainf_ckSharp.Uwp.Services.Keyboard
{
    /// <summary>
    /// The default <see langword="interface"/> for the a service that listens for keyboard strokes and shortcuts
    /// </summary>
    public interface IKeyboardListenerService
    {
        /// <summary>
        /// Raised whenever the keyboard receives a character as input
        /// </summary>
        event Action<char> CharacterReceived;
    }
}
