using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Uwp.Themes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Brainf_ckSharp.Uwp
{
    /// <summary>
    /// A <see langword="class"/> that exposes some settings in use
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Gets the current <see cref="Brainf_ckTheme"/> instance in use
        /// </summary>
        [Pure]
        public static Brainf_ckTheme GetCurrentTheme()
        {
            IdeTheme theme = Ioc.Default.GetRequiredService<ISettingsService>().GetValue<IdeTheme>(SettingsKeys.IdeTheme);

            return theme switch
            {
                IdeTheme.VisualStudio => Brainf_ckThemes.VisualStudio,
                IdeTheme.VisualStudioCode => Brainf_ckThemes.VisualStudioCode,
                IdeTheme.Monokai => Brainf_ckThemes.Monokai,
                IdeTheme.Base16 => Brainf_ckThemes.Base16,
                IdeTheme.Dracula => Brainf_ckThemes.Dracula,
                IdeTheme.OneDark => Brainf_ckThemes.OneDark,
                IdeTheme.Vim => Brainf_ckThemes.Vim,
                _ => throw new ArgumentOutOfRangeException($"Invalid theme: {theme}")
            };
        }
    }
}
