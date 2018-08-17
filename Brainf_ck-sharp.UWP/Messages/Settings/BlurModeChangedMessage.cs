using Brainf_ck_sharp_UWP.Messages.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Settings
{
    /// <summary>
    /// A simple message that signals when the current blur mode has been changed
    /// </summary>
    public sealed class BlurModeChangedMessage : ValueChangedMessageBase<int>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public BlurModeChangedMessage(int mode) : base(mode) { }
    }
}
