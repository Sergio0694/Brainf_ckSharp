namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that indicates whether or not there is code to run in the IDE
    /// </summary>
    public sealed class IDEExecutableStatusChangedMessage
    {
        /// <summary>
        /// Gets whether or not the IDE contains valid code
        /// </summary>
        public bool Executable { get; }

        /// <summary>
        /// Default constructor for a given state
        /// </summary>
        /// <param name="executable">The current IDE state</param>
        public IDEExecutableStatusChangedMessage(bool executable) => Executable = executable;
    }
}
