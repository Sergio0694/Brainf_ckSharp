using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Controls.Host;
using Brainf_ckSharp.Uwp.Helpers.UI;
using Brainf_ckSharp.Uwp.Services.Clipboard;
using Brainf_ckSharp.Uwp.Services.Files;
using Brainf_ckSharp.Uwp.Services.Keyboard;
using Brainf_ckSharp.Uwp.Services.Settings;
using Brainf_ckSharp.Uwp.Services.Share;
using GitHub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Brainf_ckSharp.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Creates a new <see cref="App"/> instance
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Initialize the UI if needed
            if (!(Window.Current.Content is Shell))
            {
                // Initialize the necessary services
                Ioc.Default.ConfigureServices(services =>
                {
                    services.AddSingleton<IFilesService, FilesService>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IKeyboardListenerService, KeyboardListenerService>();
                    services.AddSingleton<IClipboardService, ClipboardService>();
                    services.AddSingleton<IShareService, ShareService>();
                    services.AddSingleton(_ => GitHubRestFactory.GetGitHubService("Brainf_ckSharp|Uwp"));
                });
                Ioc.Default.ServiceProvider.GetRequiredService<ISettingsService>().EnsureDefaults();

                // Initial UI styling
                TitleBarHelper.ExpandViewIntoTitleBar();
                TitleBarHelper.StyleTitleBar();

                Window.Current.Content = new Shell();
            }

            // Activate the window when launching the app
            if (e.PrelaunchActivated == false)
            {
                CoreApplication.EnablePrelaunch(true);

                Window.Current.Activate();
            }
        }
    }
}
