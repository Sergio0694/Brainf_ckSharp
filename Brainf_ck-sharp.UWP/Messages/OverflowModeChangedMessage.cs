using Brainf_ck_sharp.Enums;

namespace Brainf_ck_sharp_UWP.Messages
{
    /// <summary>
    /// A message that signals whenever the current overflow mode is changed
    /// </summary>
    public sealed class OverflowModeChangedMessage
    {
        /// <summary>
        /// Gets the requested overflow mode to use in the interpreter
        /// </summary>
        public OverflowMode Mode { get; }

        /// <summary>
        /// Creates a new instance that signals the given mode
        /// </summary>
        /// <param name="mode">The selected overflow mode</param>
        public OverflowModeChangedMessage(OverflowMode mode) => Mode = mode;
    }
}
