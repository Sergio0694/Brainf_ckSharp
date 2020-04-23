using System;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
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
            return Ioc.Default.GetRequiredService<ISettingsService>().GetValue<IdeTheme>(SettingsKeys.IdeTheme) switch
            {
                IdeTheme.VisualStudio => Brainf_ckThemes.VisualStudio,
                IdeTheme.VisualStudioCode => Brainf_ckThemes.VisualStudioCode,
                IdeTheme.Monokai => Brainf_ckThemes.Monokai,
                IdeTheme.Base16 => Brainf_ckThemes.Base16,
                IdeTheme.Dracula => Brainf_ckThemes.Dracula,
                IdeTheme.OneDark => Brainf_ckThemes.OneDark,
                IdeTheme.Vim => Brainf_ckThemes.Vim,
                { } theme => throw new ArgumentOutOfRangeException($"Invalid theme: {theme}")
            };
        })();
    }
}
