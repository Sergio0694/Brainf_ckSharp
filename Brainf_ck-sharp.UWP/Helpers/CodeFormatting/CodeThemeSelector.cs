using Windows.UI;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp;

namespace Brainf_ck_sharp_UWP.Helpers.CodeFormatting
{
    public static class CodeThemeSelector
    {
        private static IDEThemeInfo _Default;

        [NotNull]
        public static IDEThemeInfo Default => _Default ?? (_Default = new IDEThemeInfo(
                                                  "#FF1E1E1E".ToColor(), "#FF333333".ToColor(), "#FF237CAD".ToColor(),
                                                  Colors.LightGray, 4, "#FF52AF3D".ToColor(),
                                                  "#FFDDDDDD".ToColor(), Colors.White, "#FF569CD6".ToColor(), Colors.IndianRed, Colors.DarkKhaki));

        private static IDEThemeInfo _Monokai;

        [NotNull]
        public static IDEThemeInfo Monokai => _Monokai ?? (_Monokai = new IDEThemeInfo(
                                                  "#FF272822".ToColor(), "#FF49483E".ToColor(), "#FFA4A59E".ToColor(),
                                                  "#FF474742".ToColor(), null, "#FFA6E22E".ToColor(),
                                                  "#FF66D9EF".ToColor(), "#FFF8F8F2".ToColor(), "#FFFD971F".ToColor(), "#FFF92672".ToColor(), "#FFAE81FF".ToColor()));

        private static IDEThemeInfo _Darkular;

        [NotNull]
        public static IDEThemeInfo Darkular => _Darkular ?? (_Darkular = new IDEThemeInfo(
                                                   "#FF282A36".ToColor(), "#FF414456".ToColor(), "#FFA5A5A6".ToColor(),
                                                   Colors.LightGray, 4, "#FF6272A4".ToColor(),
                                                   "#FFF8F8F2".ToColor(), "#FFFF79C6".ToColor(), "#FF8BE9FD".ToColor(), "#FFFFB86C".ToColor(), "#FF50FA7B".ToColor()));

        private static IDEThemeInfo _Vim;

        [NotNull]
        public static IDEThemeInfo Vim => _Vim ?? (_Vim = new IDEThemeInfo(
                                              "#FF171717".ToColor(), "#FF252525".ToColor(), "#FF727272".ToColor(),
                                              "#FF727272".ToColor(), 8, "#99B96F".ToColor(),
                                              "#FFDFDFDF".ToColor(), "#FFDFDFDF".ToColor(), "#FF999BBC".ToColor(), "#FFFFB3B2".ToColor(), "#FFFFBC77".ToColor()));
    }
}
