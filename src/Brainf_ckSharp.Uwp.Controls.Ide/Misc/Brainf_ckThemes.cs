using Windows.UI;
using Brainf_ckSharp.Uwp.Themes.Enums;
using Microsoft.Toolkit.Uwp.Helpers;

#nullable enable

namespace Brainf_ckSharp.Uwp.Themes
{
    /// <summary>
    /// A <see langword="class"/> that exposes a collection of available themes
    /// </summary>
    public static class Brainf_ckThemes
    {
        private static Brainf_ckTheme? _VisualStudio;

        /// <summary>
        /// Gets the Visual Studio theme
        /// </summary>
        public static Brainf_ckTheme VisualStudio
        {
            get
            {
                return _VisualStudio ??= new Brainf_ckTheme(
                    "#FF1E1E1E".ToColor(),
                    "#FF333333".ToColor(),
                    "#FF237CAD".ToColor(),
                    "#FF717171".ToColor(),
                    3,
                    "#FF52AF3D".ToColor(),
                    "#FFDDDDDD".ToColor(),
                    Colors.White,
                    "#FF569CD6".ToColor(),
                    Colors.IndianRed,
                    Colors.DarkKhaki,
                    "#FF009191".ToColor(),
                    "#FF1E7499".ToColor(),
                    LineHighlightStyle.Outline,
                    "#30FFFFFF".ToColor(),
                    "Visual Studio");
            }
        }
    }
}
