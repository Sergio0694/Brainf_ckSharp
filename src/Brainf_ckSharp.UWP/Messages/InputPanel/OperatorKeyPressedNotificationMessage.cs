using Brainf_ckSharp.UWP.Messages.Abstract;

namespace Brainf_ckSharp.UWP.Messages.InputPanel
{
    /// <summary>
    /// A message that notifies whenever a key for a Brainf*ck/PBrain operator is pressed
    /// </summary>
    public sealed class OperatorKeyPressedNotificationMessage : ValueChangedMessageBase<char>
    {
        /// <summary>
        /// Creates a new <see cref="OperatorKeyPressedNotificationMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The input operator</param>
        public OperatorKeyPressedNotificationMessage(char value) : base(value) { }
    }
}
