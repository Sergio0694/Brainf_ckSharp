using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Uwp.Messages.Navigation
{
    /// <summary>
    /// A message that indicates whenever the app global loading state changes
    /// </summary>
    public sealed class LoadingStateUpdateNotificationMessage : ValueChangedMessage<bool>
    {
        /// <summary>
        /// Creates a new <see cref="LoadingStateUpdateNotificationMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The new loading state for the current application</param>
        public LoadingStateUpdateNotificationMessage(bool value) : base(value) { }
    }
}
