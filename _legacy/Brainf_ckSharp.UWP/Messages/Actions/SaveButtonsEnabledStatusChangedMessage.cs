namespace Brainf_ck_sharp.Legacy.UWP.Messages.Actions
{
    /// <summary>
    /// Indicates a change in the enabled status for the save buttons
    /// </summary>
    public class SaveButtonsEnabledStatusChangedMessage
    {
        /// <summary>
        /// Gets whether or not there's a source code already loaded that can be saved
        /// </summary>
        public bool SaveEnabled { get; }

        /// <summary>
        /// Gets whether or not the current source code can be saved as a separate file
        /// </summary>
        public bool SaveAsEnabled { get; }

        /// <summary>
        /// Creates a new instance with the given status for the two buttons
        /// </summary>
        /// <param name="save">Indicates whether there's a source code loaded that can be saved</param>
        /// <param name="saveAs">Indicates whether or not the current code can be saved with a name</param>
        public SaveButtonsEnabledStatusChangedMessage(bool save, bool saveAs)
        {
            SaveEnabled = save;
            SaveAsEnabled = saveAs;
        }
    }
}
