using Brainf_ck_sharp.Legacy.UWP.Messages.Abstract;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.Actions
{
    /// <summary>
    /// Indicates whether or not there are available breakpoints in the current source code being edited
    /// </summary>
    public sealed class DebugStatusChangedMessage : ValueChangedMessageBase<bool>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public DebugStatusChangedMessage(bool debug) : base(debug) {}
    }
}
