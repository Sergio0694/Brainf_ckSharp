using Brainf_ck_sharp_UWP.Messages.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.IDE
{
    /// <summary>
    /// A message that indicates whether the latest requested breakpoint could be created successfully
    /// </summary>
    public sealed class BreakpointErrorStatusChangedMessage : ValueChangedMessageBase<bool>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public BreakpointErrorStatusChangedMessage(bool valid) : base(valid) { }
    }
}
