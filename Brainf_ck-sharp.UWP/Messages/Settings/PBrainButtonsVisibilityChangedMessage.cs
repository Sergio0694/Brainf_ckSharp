using Brainf_ck_sharp_UWP.Messages.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Settings
{
    /// <summary>
    /// A message that notifies whenever the user changes the visibility setting for the PBrain buttons
    /// </summary>
    public sealed class PBrainButtonsVisibilityChangedMessage : ValueChangedMessageBase<bool>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public PBrainButtonsVisibilityChangedMessage(bool visible) : base(visible) { }
    }
}
