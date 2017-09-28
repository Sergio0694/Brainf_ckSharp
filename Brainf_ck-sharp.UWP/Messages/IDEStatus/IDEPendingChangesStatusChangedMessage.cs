namespace Brainf_ck_sharp_UWP.Messages.IDEStatus
{
    /// <summary>
    /// A message that indicates that the IDE pending changes status has been modified by the user
    /// </summary>
    public sealed class IDEPendingChangesStatusChangedMessage
    {
        /// <summary>
        /// Gets whether or not there are pending changes in the IDE
        /// </summary>
        public bool PendingChangesPresent { get; }

        /// <summary>
        /// Creates a new message that wraps the input status
        /// </summary>
        /// <param name="pending">The desired status for the message</param>
        public IDEPendingChangesStatusChangedMessage(bool pending) => PendingChangesPresent = pending;
    }
}
