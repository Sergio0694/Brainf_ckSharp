using System;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Uwp.Themes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Brainf_ckSharp.Uwp.Constants
{
    /// <summary>
    /// A <see langword="class"/> with some readonly settings that are initialized during startup
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Gets whether or not the extended post thumbnails are enabled
        /// </summary>
        public static readonly Brainf_ckTheme Brainf_ckTheme = new Func<Brainf_ckTheme>(() =>
        {
            return Ioc.Default.GetRequiredService<ISettingsService>().GetValue<int>(SettingsKeys.Theme) switch
            {
                0 => Brainf_ckThemes.VisualStudio,
                1 => Brainf_ckThemes.Monokai,
                2 => Brainf_ckThemes.Dracula,
                3 => Brainf_ckThemes.Base16,
                4 => Brainf_ckThemes.OneDark,
                5 => Brainf_ckThemes.Vim,
                6 => Brainf_ckThemes.VisualStudioCode,
                { } i => throw new ArgumentOutOfRangeException($"Invalid theme index: {i}")
            };
        })();
    }
}
