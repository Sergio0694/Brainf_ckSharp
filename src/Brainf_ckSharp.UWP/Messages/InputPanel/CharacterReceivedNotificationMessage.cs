using Brainf_ckSharp.UWP.Messages.Abstract;

namespace Brainf_ckSharp.UWP.Messages.InputPanel
{
    /// <summary>
    /// A messaged that signals whenever the user inputs a character into the app, regardless of current focus
    /// </summary>
    public sealed class CharacterReceivedNotificationMessage : ValueChangedMessageBase<char>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public CharacterReceivedNotificationMessage(char c) : base(c) { }
    }
}
