using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static IDEThemeInfo Default { get; } = _Default ?? (_Default = new IDEThemeInfo(
                                                          "#FF1E1E1E".ToColor(), "#FF333333".ToColor(), "#FF52AF3D".ToColor(),
                                                          "#FFDDDDDD".ToColor(), Colors.White, "#FF569CD6".ToColor(), Colors.IndianRed, Colors.DarkKhaki));
    }
}
