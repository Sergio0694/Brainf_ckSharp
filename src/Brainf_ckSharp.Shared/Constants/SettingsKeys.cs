namespace Brainf_ckSharp.Shared.Constants
{
    /// <summary>
    /// A <see langword="class"/> that holds the various setting keys used in the app
    /// </summary>
    public static class SettingsKeys
    {
        /// <summary>
        /// Indicates whether or not to autosave documents, uses a <see cref="bool"/>
        /// </summary>
        public const string AutosaveDocuments = nameof(AutosaveDocuments);

        /// <summary>
        /// Indicates whether to protect from closing the app with unsaved changes, uses a <see cref="bool"/>
        /// </summary>
        public const string ProtectUnsavedChanges = nameof(ProtectUnsavedChanges);

        /// <summary>
        /// Indicates whether or not to automatically indent brackets in the IDE, uses a <see cref="bool"/>
        /// </summary>
        public const string AutoindentBrackets = nameof(AutoindentBrackets);

        /// <summary>
        /// Indicates whether to place open brackets on a new line, uses a <see cref="bool"/>
        /// </summary>
        public const string BracketsOnNewLine = nameof(BracketsOnNewLine);

        /// <summary>
        /// Indicates the tab length, uses an <see cref="int"/>
        /// </summary>
        public const string TabLength = nameof(TabLength);

        /// <summary>
        /// Indicates the index of the current syntax highlight theme, uses an <see cref="int"/>
        /// </summary>
        public const string Theme = nameof(Theme);

        /// <summary>
        /// Indicates whether to render whitespace characters, uses a <see cref="bool"/>
        /// </summary>
        public const string RenderWhitespaces = nameof(RenderWhitespaces);

        /// <summary>
        /// Indicates whether to enable the Windows timeline, uses a <see cref="bool"/>
        /// </summary>
        public const string EnableTimeline = nameof(EnableTimeline);

        /// <summary>
        /// Indicates the index of the starting page, uses an <see cref="int"/>
        /// </summary>
        public const string StartingPage = nameof(StartingPage);

        /// <summary>
        /// Indicates whether or not to automatically clear the stdin buffer whenever it's requested, uses a <see cref="bool"/>
        /// </summary>
        public const string ClearStdinBufferOnRequest = nameof(ClearStdinBufferOnRequest);

        /// <summary>
        /// Indicates whether or not to show the PBrain buttons, uses a <see cref="bool"/>
        /// </summary>
        public const string ShowPBrainButtons = nameof(ShowPBrainButtons);

        /// <summary>
        /// Indicates the overflow mode currently in use, uses a <see cref="Brainf_ckSharp.Enums.OverflowMode"/>
        /// </summary>
        public const string OverflowMode = nameof(OverflowMode);

        /// <summary>
        /// Indicates the memory size for the interpreter, uses an <see cref="int"/>
        /// </summary>
        public const string MemorySize = nameof(MemorySize);
    }
}
