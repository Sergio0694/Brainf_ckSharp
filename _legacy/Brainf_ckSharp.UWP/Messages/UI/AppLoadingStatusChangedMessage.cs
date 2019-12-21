namespace Brainf_ck_sharp.Legacy.UWP.Messages.UI
{
    /// <summary>
    /// A message that indicates whether or not the app is loading and its content should be blocked
    /// </summary>
    public sealed class AppLoadingStatusChangedMessage
    {
        /// <summary>
        /// Gets whether or not the app is currently loading
        /// </summary>
        public bool Loading { get; }

        /// <summary>
        /// Gets whether or not to show the loading UI without playing animations
        /// </summary>
        public bool ImmediateDisplayRequested { get; }

        /// <summary>
        /// Creates a new instance for a given state
        /// </summary>
        /// <param name="status">The updated loading status</param>
        /// <param name="immediate">Indicates whether or not to skip the animations</param>
        public AppLoadingStatusChangedMessage(bool status, bool immediate = false)
        {
            Loading = status;
            ImmediateDisplayRequested = immediate;
        }
    }
}
