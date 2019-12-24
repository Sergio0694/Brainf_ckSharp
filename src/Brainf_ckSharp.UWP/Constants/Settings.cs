using System;
using Brainf_ckSharp.UWP.Models.Themes;
using Brainf_ckSharp.UWP.Services.Settings;
using GalaSoft.MvvmLight.Ioc;

namespace Brainf_ckSharp.UWP.Constants
{
    /// <summary>
    /// A <see langword="class"/> with some readonly settings that are initialized during startup
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Gets whether or not the extended post thumbnails are enabled
        /// </summary>
        public static readonly ThemeInfo Theme = new Func<ThemeInfo>(() =>
        {
            return SimpleIoc.Default.GetInstance<ISettingsService>().GetValue<int>(SettingsKeys.Theme) switch
            {
                0 => Themes.Default,
                1 => Themes.Monokai,
                2 => Themes.Dracula,
                3 => Themes.Base16,
                4 => Themes.OneDark,
                5 => Themes.Vim,
                6 => Themes.VisualStudioCode,
                { } i => throw new ArgumentOutOfRangeException($"Invalid theme index: {i}")
            };
        })();
    }
}
