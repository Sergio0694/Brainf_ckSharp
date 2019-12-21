using Windows.UI;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Brainf_ck_sharp.Legacy.UWP.Helpers.CodeFormatting
{
    /// <summary>
    /// A simpla class that initializes and holds the list of available IDE themes
    /// </summary>
    public static class CodeThemes
    {
        private static IDEThemeInfo _Default;

        /// <summary>
        /// Gets the original IDE theme for the app
        /// </summary>
        [NotNull]
        public static IDEThemeInfo Default => _Default ?? (_Default = new IDEThemeInfo(
                                                  "#FF1E1E1E".ToColor(), "#FF333333".ToColor(), "#FF237CAD".ToColor(),
                                                  "#FF717171".ToColor(), 3, "#FF52AF3D".ToColor(),
                                                  "#FFDDDDDD".ToColor(), Colors.White, "#FF569CD6".ToColor(), Colors.IndianRed, Colors.DarkKhaki,
                                                  "#FF009191".ToColor(), "#FF1E7499".ToColor(),
                                                  LineHighlightStyle.Outline, "#30FFFFFF".ToColor(), LocalizationManager.GetResource("Default")));

        private static IDEThemeInfo _Monokai;

        /// <summary>
        /// Gets the Monokai theme inspired to Atom and Sublime Text
        /// </summary>
        [NotNull]
        public static IDEThemeInfo Monokai => _Monokai ?? (_Monokai = new IDEThemeInfo(
                                                  "#FF272822".ToColor(), "#FF49483E".ToColor(), "#FFA4A59E".ToColor(),
                                                  "#FF474742".ToColor(), null, "#FFA6E22E".ToColor(),
                                                  "#FF66D9EF".ToColor(), "#FFF8F8F2".ToColor(), "#FFFD971F".ToColor(), "#FFF92672".ToColor(), "#FFAE81FF".ToColor(),
                                                  "#FFB56741".ToColor(), "#FF9B8FB2".ToColor(),
                                                  LineHighlightStyle.Fill, "#FF515151".ToColor()));

        private static IDEThemeInfo _Dracula;

        /// <summary>
        /// Gets the Dracula theme, ispired to the same theme available for Atom and Sublime Text
        /// </summary>
        [NotNull]
        public static IDEThemeInfo Dracula => _Dracula ?? (_Dracula = new IDEThemeInfo(
                                                  "#FF282A36".ToColor(), "#FF414456".ToColor(), "#FFA5A5A6".ToColor(),
                                                  Colors.LightGray, 4, "#FF6272A4".ToColor(),
                                                  "#FFF8F8F2".ToColor(), "#FFFF79C6".ToColor(), "#FF8BE9FD".ToColor(), "#FFFFB86C".ToColor(), "#FF50FA7B".ToColor(),
                                                  "#FF527CFF".ToColor(), "#FF8C88CE".ToColor(),
                                                  LineHighlightStyle.Fill, "#FF353746".ToColor()));

        private static IDEThemeInfo _Vim;

        /// <summary>
        /// Gets the Vim from the old-school code editor
        /// </summary>
        [NotNull]
        public static IDEThemeInfo Vim => _Vim ?? (_Vim = new IDEThemeInfo(
                                              "#FF171717".ToColor(), "#FF252525".ToColor(), "#FF727272".ToColor(),
                                              "#FF646464".ToColor(), 6, "#FF99B96F".ToColor(),
                                              "#FFDFDFDF".ToColor(), "#FFDFDFDF".ToColor(), "#FF999BBC".ToColor(), "#FFFFB3B2".ToColor(), "#FFFFBC77".ToColor(),
                                              "#FF826880".ToColor(), "#FF917575".ToColor(),
                                              LineHighlightStyle.Fill, "#FF222222".ToColor()));

        private static IDEThemeInfo _OneDark;

        /// <summary>
        /// Gets the Vim from the old-school code editor
        /// </summary>
        [NotNull]
        public static IDEThemeInfo OneDark => _OneDark ?? (_OneDark = new IDEThemeInfo(
                                                  "#FF282C34".ToColor(), "#FF383E49".ToColor(), "#FF5A5A5A".ToColor(),
                                                  "#FF3C4049".ToColor(), null, "#FF5C6370".ToColor(),
                                                  "#FFC0C0C0".ToColor(), "#FF56B6C2".ToColor(), "#FFABB2BF".ToColor(), "#FFD19A66".ToColor(), "#FFE06C75".ToColor(),
                                                  "#FFBC794F".ToColor(), "#FFB5544C".ToColor(),
                                                  LineHighlightStyle.Fill, "#FF363A4F".ToColor(), "One Dark"));

        private static IDEThemeInfo _Base16;

        /// <summary>
        /// Gets the Vim from the old-school code editor
        /// </summary>
        [NotNull]
        public static IDEThemeInfo Base16 => _Base16 ?? (_Base16 = new IDEThemeInfo(
                                                  "#FF1D1F21".ToColor(), "#FF373B41".ToColor(), "#FF656767".ToColor(),
                                                  "#FF373B41".ToColor(), null, "#FF969896".ToColor(),
                                                  "#FFE0935A".ToColor(), "#FFC5C8C6".ToColor(), "#FFB393BC".ToColor(), "#FFB5BE63".ToColor(), "#FFCE6564".ToColor(),
                                                  "#FF357199".ToColor(), "#FF69789E".ToColor(),
                                                  LineHighlightStyle.Fill, "#FF2E3032".ToColor(), "Base 16"));

        private static IDEThemeInfo _VisualStudioCode;

        /// <summary>
        /// Gets the Vim from the old-school code editor
        /// </summary>
        [NotNull]
        public static IDEThemeInfo VisualStudioCode => _VisualStudioCode ?? (_VisualStudioCode = new IDEThemeInfo(
                                                 "#FF1E1E1E".ToColor(), "#FF252526".ToColor(), "#FF4B4B4B".ToColor(),
                                                 "#FF404040".ToColor(), null, "#FF4B8B43".ToColor(),
                                                 "#FF68D0FE".ToColor(), "#FFD4D4D4".ToColor(), "#FF31BEB0".ToColor(), "#FFC3622C".ToColor(), "#FFD0A641".ToColor(),
                                                 "#FF437AC1".ToColor(), "#FF2F9187".ToColor(),
                                                 LineHighlightStyle.Outline, "#20FFFFFF".ToColor(), "Visual Studio Code"));
    }
}
