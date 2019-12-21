using Brainf_ck_sharp.Legacy.UWP.Messages.Abstract;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.IDE
{
    /// <summary>
    /// A message that indicates that the IDE pending changes status has been modified by the user
    /// </summary>
    public sealed class IDEPendingChangesStatusChangedMessage : ValueChangedMessageBase<bool>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public IDEPendingChangesStatusChangedMessage(bool pending) : base(pending) { }
    }
}
