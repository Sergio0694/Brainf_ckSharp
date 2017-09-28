namespace Brainf_ck_sharp_UWP.Messages.UI
{
    /// <summary>
    /// A simple message that signals when the current blur mode has been changed
    /// </summary>
    public sealed class BlurModeChangedMessage
    {
        /// <summary>
        /// Gets the new blur mode to use
        /// </summary>
        public int BlurMode { get; }

        /// <summary>
        /// Creates a new instance that wraps the new blur mode
        /// </summary>
        /// <param name="mode">The new blur mode to apply</param>
        public BlurModeChangedMessage(int mode) => BlurMode = mode;
    }
}
