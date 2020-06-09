using Brainf_ck_sharp.Legacy.UWP.Messages.Abstract;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.Actions
{
    /// <summary>
    /// A message that indicates whether or not there is code to run in the IDE
    /// </summary>
    public sealed class IDEExecutableStatusChangedMessage : ValueChangedMessageBase<bool>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public IDEExecutableStatusChangedMessage(bool executable) : base(executable) { }
    }
}
