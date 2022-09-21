namespace Brainf_ckSharp.Shared.Constants;

/// <summary>
/// A <see langword="class"/> that holds the various setting keys used in the app
/// </summary>
public static class SettingsKeys
{
    /// <summary>
    /// Gets whether or not the virtual Brainf*ck/PBrain keyboard is enabled, uses a <see cref="bool"/>
    /// </summary>
    public const string IsVirtualKeyboardEnabled = nameof(IsVirtualKeyboardEnabled);

    /// <summary>
    /// Indicates whether to place open brackets on a new line, uses a <see cref="Enums.Settings.BracketsFormattingStyle"/>
    /// </summary>
    public const string BracketsFormattingStyle = nameof(BracketsFormattingStyle);

    /// <summary>
    /// Indicates the index of the current syntax highlight theme, uses an <see cref="Enums.Settings.IdeTheme"/>
    /// </summary>
    public const string IdeTheme = nameof(IdeTheme);

    /// <summary>
    /// Indicates whether to render whitespace characters, uses a <see cref="bool"/>
    /// </summary>
    public const string RenderWhitespaces = nameof(RenderWhitespaces);

    /// <summary>
    /// Indicates the index of the selected view, uses a <see cref="Enums.Settings.ViewType"/>
    /// </summary>
    public const string SelectedView = nameof(SelectedView);

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
