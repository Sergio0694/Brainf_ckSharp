using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums.Settings;
using Microsoft.Toolkit.Diagnostics;

namespace Brainf_ckSharp.Uwp.Themes
{
    /// <summary>
    /// An extension class to interop with <see cref="Brainf_ckThemes"/>
    /// </summary>
    public static class Brainf_ckThemesExtensions
    {
        /// <summary>
        /// Gets the current <see cref="Brainf_ckTheme"/> instance in use
        /// </summary>
        [Pure]
        public static Brainf_ckTheme AsBrainf_ckTheme(this IdeTheme theme)
        {
            return theme switch
            {
                IdeTheme.VisualStudio => Brainf_ckThemes.VisualStudio,
                IdeTheme.VisualStudioCode => Brainf_ckThemes.VisualStudioCode,
                IdeTheme.Monokai => Brainf_ckThemes.Monokai,
                IdeTheme.Base16 => Brainf_ckThemes.Base16,
                IdeTheme.XCodeDark => Brainf_ckThemes.XCodeDark,
                IdeTheme.Dracula => Brainf_ckThemes.Dracula,
                IdeTheme.OneDark => Brainf_ckThemes.OneDark,
                IdeTheme.Vim => Brainf_ckThemes.Vim,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<Brainf_ckTheme>("Invalid requested theme")
            };
        }
    }
}
