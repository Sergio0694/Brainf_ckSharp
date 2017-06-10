namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever an action for the console changes its status
    /// </summary>
    public class ConsoleAvailableActionStatusChangedMessage
    {
        /// <summary>
        /// Gets the current action that changed its status
        /// </summary>
        public ConsoleAction Action { get; }

        /// <summary>
        /// Gets the new status for the action
        /// </summary>
        public bool Status { get; }

        /// <summary>
        /// Creates a new instance for the given action
        /// </summary>
        /// <param name="action">The current action</param>
        /// <param name="status">The new status for the action</param>
        public ConsoleAvailableActionStatusChangedMessage(ConsoleAction action, bool status)
        {
            Action = action;
            Status = status;
        }
    }

    /// <summary>
    /// Indicates a console action that can be performed
    /// </summary>
    public enum ConsoleAction
    {
        Play,
        Restart,
        Clear,
        Undo
    }
}
