namespace Brainf_ck_sharp_UWP.Messages.UI
{
    /// <summary>
    /// An empty message that signals that the IDE theme has been changed and the UI needs a refresh
    /// </summary>
    public sealed class IDESettingsChangedMessage
    {
        /// <summary>
        /// Gets whether or not the IDE theme has been changed
        /// </summary>
        public bool ThemeChanged { get; }

        /// <summary>
        /// Gets whether or not the IDE tab length setting has been changed
        /// </summary>
        public bool TabsLengthChanged { get; }

        /// <summary>
        /// Gets whether or not the IDE render whitespaces setting has been changed
        /// </summary>
        public bool WhitespacesChanged { get; }

        /// <summary>
        /// Creates a new instance for a setting update event
        /// </summary>
        /// <param name="themeChanged">Indicates whether or not the theme has been changed</param>
        /// <param name="tabsChanged">Indicates whether or not the tab length setting has been changed</param>
        /// <param name="whitespacesChanged">Indicates whether the IDE render whitespaces setting has been changed</param>
        public IDESettingsChangedMessage(bool themeChanged, bool tabsChanged, bool whitespacesChanged)
        {
            ThemeChanged = themeChanged;
            TabsLengthChanged = tabsChanged;
            WhitespacesChanged = whitespacesChanged;
        }
    }
}
