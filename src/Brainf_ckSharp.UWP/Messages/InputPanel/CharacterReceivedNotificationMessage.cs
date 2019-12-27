using Brainf_ckSharp.Uwp.Messages.Abstract;

namespace Brainf_ckSharp.Uwp.Messages.InputPanel
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
