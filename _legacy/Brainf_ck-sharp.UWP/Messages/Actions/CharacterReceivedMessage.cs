using Brainf_ck_sharp_UWP.Messages.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A messaged that signals whenever the user inputs a character into the app, regardless of current focus
    /// </summary>
    public sealed class CharacterReceivedMessage : ValueChangedMessageBase<char>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public CharacterReceivedMessage(char c) : base(c) { }
    }
}
