namespace Brainf_ckSharp.Uwp.Services.Keyboard
{
    /// <summary>
    /// The default <see langword="interface"/> for the a service that listens for keyboard strokes and shortcuts
    /// </summary>
    public interface IKeyboardListenerService
    {
        /// <summary>
        /// Gets or sets whether or not the current instance is monitoring the keyboard events
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
