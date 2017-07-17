namespace Brainf_ck_sharp_UWP.Messages.UI
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
        /// Creates a new instance for a given state
        /// </summary>
        /// <param name="status">The updated loading status</param>
        public AppLoadingStatusChangedMessage(bool status) => Loading = status;
    }
}
