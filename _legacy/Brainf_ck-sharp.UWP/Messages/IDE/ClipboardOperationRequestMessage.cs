using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Messages.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.IDE
{
    /// <summary>
    /// A message that signals whenever the user requests to perform a specific clipboard operation
    /// </summary>
    public sealed class ClipboardOperationRequestMessage : ValueChangedMessageBase<ClipboardOperation>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public ClipboardOperationRequestMessage(ClipboardOperation operation) : base(operation) { }
    }
}
