using Brainf_ckSharp.UWP.Models.Themes;

namespace Brainf_ckSharp.UWP.Helpers
{
    /// <summary>
    /// A small <see langword="class"/> with some app constants
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// A <see langword="class"/> with some readonly settings that are initialized during startup
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// Gets whether or not the extended post thumbnails are enabled
            /// </summary>
            public static readonly ThemeInfo Theme;
        }
    }
}
