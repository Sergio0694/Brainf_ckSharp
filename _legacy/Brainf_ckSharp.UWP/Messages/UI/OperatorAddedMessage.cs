using Brainf_ck_sharp.Legacy.UWP.Messages.Abstract;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.UI
{
    /// <summary>
    /// A simple message that signals when a new operator is being added by the user
    /// </summary>
    public sealed class OperatorAddedMessage : ValueChangedMessageBase<char>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public OperatorAddedMessage(char @operator) : base(@operator) { }
    }
}
