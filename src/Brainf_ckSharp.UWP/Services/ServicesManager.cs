using Brainf_ckSharp.UWP.Services.Keyboard;
using Brainf_ckSharp.UWP.Services.Settings;
using GalaSoft.MvvmLight.Ioc;
using GitHub;

namespace Brainf_ckSharp.UWP.Services
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
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<IKeyboardListenerService, KeyboardListenerService>();
            SimpleIoc.Default.Register(() => GitHubRestFactory.GetGitHubService("Brainf_ckSharp|UWP"));
        }
    }
}
