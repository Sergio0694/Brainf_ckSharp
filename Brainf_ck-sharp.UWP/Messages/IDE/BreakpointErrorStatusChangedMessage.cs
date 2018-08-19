namespace Brainf_ck_sharp_UWP.Messages.IDE
{
    /// <summary>
    /// A message that indicates whether the latest requested breakpoint could be created successfully
    /// </summary>
    public sealed class BreakpointErrorStatusChangedMessage
    {
        /// <summary>
        /// Gets the result status for the latest added breakpoint
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Creates a new message that wraps the input status
        /// </summary>
        /// <param name="valid">The desired status for the message</param>
        public BreakpointErrorStatusChangedMessage(bool valid) => IsValid = valid;
    }
}
