namespace Brainf_ck_sharp_UWP.Messages.UI
{
    /// <summary>
    /// A message that notifies whenever the user changes the visibility setting for the PBrain buttons
    /// </summary>
    public sealed class PBrainButtonsVisibilityChangedMessage
    {
        /// <summary>
        /// Gets whether or not the PBrain buttons should be visible
        /// </summary>
        public bool Visible { get; }

        /// <summary>
        /// Creates a new instance to notify the UI
        /// </summary>
        /// <param name="visible">The new visibility value</param>
        public PBrainButtonsVisibilityChangedMessage(bool visible) => Visible = visible;
    }
}
