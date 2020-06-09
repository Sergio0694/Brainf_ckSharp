using Brainf_ck_sharp.Legacy.UWP.Messages.Abstract;
using Brainf_ckSharp.Legacy.Enums;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.Settings
{
    /// <summary>
    /// A message that signals whenever the current overflow mode is changed
    /// </summary>
    public sealed class OverflowModeChangedMessage : ValueChangedMessageBase<OverflowMode>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public OverflowModeChangedMessage(OverflowMode mode) : base(mode) { }
    }
}
