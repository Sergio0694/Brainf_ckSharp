using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Brainf_ckSharp.UWP.Messages.InputPanel;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ckSharp.UWP.Services.Keyboard
{
    /// <summary>
    /// A <see langword="class"/> that listens to the global <see cref="UIElement.CharacterReceived"/> event for the current app window
    /// </summary>
    public sealed class KeyboardListenerService : IKeyboardListenerService
    {
        private bool _IsEnabled;

        /// <inheritdoc/>
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (IsEnabled != value)
                {
                    if (value) Window.Current.Content.CharacterReceived += Content_CharacterReceived;
                    else Window.Current.Content.CharacterReceived -= Content_CharacterReceived;
                    _IsEnabled = value;
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
            Messenger.Default.Send(new CharacterReceivedNotificationMessage(args.Character));
        }
    }
}
