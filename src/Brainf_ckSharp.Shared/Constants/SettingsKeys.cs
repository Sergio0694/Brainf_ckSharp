namespace Brainf_ckSharp.Shared.Constants
{
    /// <summary>
    /// A <see langword="class"/> that holds the various setting keys used in the app
    /// </summary>
    public static class SettingsKeys
    {
        /// <summary>
        /// Indicates the index of the current syntax highlight theme, uses an <see cref="int"/>
        /// </summary>
        public const string Theme = nameof(Theme);

        /// <summary>
        /// Indicates whether or not to automatically clear the stdin buffer whenever it's requested, uses a <see cref="bool"/>
        /// </summary>
        public const string ClearStdinBufferOnRequest = nameof(ClearStdinBufferOnRequest);
    }
}
