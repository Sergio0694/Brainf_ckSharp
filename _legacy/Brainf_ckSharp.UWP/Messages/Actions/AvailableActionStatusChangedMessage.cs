namespace Brainf_ck_sharp.Legacy.UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever an action for the console changes its status
    /// </summary>
    public class AvailableActionStatusChangedMessage
    {
        /// <summary>
        /// Gets the current action that changed its status
        /// </summary>
        public SharedAction Action { get; }

        /// <summary>
        /// Gets the new status for the action
        /// </summary>
        public bool Status { get; }

        /// <summary>
        /// Creates a new instance for the given action
        /// </summary>
        /// <param name="action">The current action</param>
        /// <param name="status">The new status for the action</param>
        public AvailableActionStatusChangedMessage(SharedAction action, bool status)
        {
            Action = action;
            Status = status;
        }
    }

    /// <summary>
    /// Indicates an action that can be performed either in the console or in the IDE
    /// </summary>
    public enum SharedAction
    {
        Play,
        Restart,
        Clear,
        DeleteLastCharacter,
        ClearScreen,
        RepeatLastScript,
        Delete,
        Undo,
        Redo
    }
}
