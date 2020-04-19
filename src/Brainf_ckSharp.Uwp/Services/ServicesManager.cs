using Brainf_ckSharp.Uwp.Services.Clipboard;
using Brainf_ckSharp.Uwp.Services.Files;
using Brainf_ckSharp.Uwp.Services.Keyboard;
using Brainf_ckSharp.Uwp.Services.Settings;
using Brainf_ckSharp.Uwp.Services.Share;
using GitHub;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Brainf_ckSharp.Uwp.Services
{
    /// <summary>
    /// A <see langword="class"/> that manages the various IoC services used in the app
    /// </summary>
    public static class ServicesManager
    {
        /// <summary>
        /// Initializes the available app services
        /// </summary>
        public static void InitializeServices()
        {
            Ioc.Default.Register<IFilesService, FilesService>();
            Ioc.Default.Register<ISettingsService, SettingsService>();
            Ioc.Default.Register<IKeyboardListenerService, KeyboardListenerService>();
            Ioc.Default.Register<IClipboardService, ClipboardService>();
            Ioc.Default.Register<IShareService, ShareService>();
            Ioc.Default.Register(() => GitHubRestFactory.GetGitHubService("Brainf_ckSharp|Uwp"));
        }
    }
}
