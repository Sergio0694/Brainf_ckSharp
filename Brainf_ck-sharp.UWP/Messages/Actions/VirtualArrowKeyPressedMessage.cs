using Brainf_ck_sharp_UWP.Messages.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever the user presses a key in the virtual arrows keyboard
    /// </summary>
    public class VirtualArrowKeyPressedMessage : ValueChangedMessageBase<VirtualArrow>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public VirtualArrowKeyPressedMessage(VirtualArrow direction) : base(direction) { }
    }

    /// <summary>
    /// Indicates a key in the virtual arrows keyboard
    /// </summary>
    public enum VirtualArrow
    {
        Up,
        Left,
        Down,
        Right
    }
}
