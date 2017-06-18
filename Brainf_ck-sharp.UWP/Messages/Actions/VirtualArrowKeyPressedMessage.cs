namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever the user presses a key in the virtual arrows keyboard
    /// </summary>
    public class VirtualArrowKeyPressedMessage
    {
        /// <summary>
        /// Gets the pressed direction key
        /// </summary>
        public VirtualArrow Direction { get; }

        /// <summary>
        /// Creates a new instance for the given direction
        /// </summary>
        /// <param name="direction">The desired direction</param>
        public VirtualArrowKeyPressedMessage(VirtualArrow direction) => Direction = direction;
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
