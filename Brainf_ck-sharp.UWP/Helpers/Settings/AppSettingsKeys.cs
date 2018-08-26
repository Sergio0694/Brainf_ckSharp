namespace Brainf_ck_sharp_UWP.Helpers.Settings
{
    /// <summary>
    /// A collection of keys of application settings
    /// </summary>
    public enum AppSettingsKeys
    {
        // General settings
        WelcomeMessageShown,
        OverflowToggleMessageShown,
        ByteOverflowModeEnabled,
        ReviewPromptShown,
        AppStartups,

        // IDE settings
        AutosaveDocuments,
        AutoIndentBrackets,
        BracketsStyle,
        TabLength,
        SelectedIDETheme,
        RenderWhitespaces,
        SelectedFontName,
        EnableTimeline,

        // Other settings
        StartingPage,
        ClearStdinBufferOnExecution,
        ShowPBrainButtons,
        ProtectUnsavedChanges,
        AutorunCodeInBackground,
        InterpreterMemorySize
    }
}