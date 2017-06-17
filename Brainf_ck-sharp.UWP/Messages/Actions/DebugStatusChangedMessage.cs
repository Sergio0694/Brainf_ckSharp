namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// Indicates whether or not there are available breakpoints in the current source code being edited
    /// </summary>
    public sealed class DebugStatusChangedMessage
    {
        /// <summary>
        /// Gets whether or not the debug mode can be activated
        /// </summary>
        public bool DebugAvailable { get; }

        /// <summary>
        /// Default constructor for a given debug state
        /// </summary>
        /// <param name="debug">Indicates whether or not there are any breakpoints in the IDE</param>
        public DebugStatusChangedMessage(bool debug) => DebugAvailable = debug;
    }
}
