using Brainf_ck_sharp_UWP.Helpers.CodeFormatting;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.UI
{
    /// <summary>
    /// A message that signals whenever the user changes the current font selection
    /// </summary>
    public sealed class IDEThemePreviewFontChangedMessage
    {
        /// <summary>
        /// Gets the new selected font to use
        /// </summary>
        [NotNull]
        public InstalledFont Font { get; }

        /// <summary>
        /// Creates a new instance around the given font
        /// </summary>
        /// <param name="font">The new font selected by the user</param>
        public IDEThemePreviewFontChangedMessage([NotNull] InstalledFont font) => Font = font;
    }
}
