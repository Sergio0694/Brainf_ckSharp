using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Uwp.Controls.Host;
using Brainf_ckSharp.Uwp.Helpers;
using Brainf_ckSharp.Uwp.Services.Clipboard;
using Brainf_ckSharp.Uwp.Services.Files;
using Brainf_ckSharp.Uwp.Services.Keyboard;
using Brainf_ckSharp.Uwp.Services.Settings;
using Brainf_ckSharp.Uwp.Services.Share;
using GitHub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Brainf_ckSharp.Services.Uwp.Store;

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
                ConfigureServices();

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

        /// <summary>
        /// Performs additional settings configuration and other startup initialization
        /// </summary>
        private static void ConfigureServices()
        {
            // Default services
            Ioc.Default.ConfigureServices(services =>
            {
                services.AddSingleton<IFilesService, FilesService>();
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<IKeyboardListenerService, KeyboardListenerService>();
                services.AddSingleton<IClipboardService, ClipboardService>();
                services.AddSingleton<IShareService, ShareService>();
                services.AddSingleton(_ => GitHubRestFactory.GetGitHubService("Brainf_ckSharp|Uwp"));
#if DEBUG
                services.AddSingleton<IStoreService, TestStoreService>();
#else
                services.AddSingleton<IStoreService, ProductionStoreService>();
#endif
            });

            ISettingsService settings = Ioc.Default.GetRequiredService<ISettingsService>();

            // Initialize default settings
            settings.SetValue(SettingsKeys.AutosaveDocuments, false, false);
            settings.SetValue(SettingsKeys.ProtectUnsavedChanges, false, false);
            settings.SetValue(SettingsKeys.AutoindentBrackets, true, false);
            settings.SetValue(SettingsKeys.BracketsFormattingStyle, BracketsFormattingStyle.NewLine, false);
            settings.SetValue(SettingsKeys.TabLength, 4, false);
            settings.SetValue(SettingsKeys.IdeTheme, IdeTheme.VisualStudio, false);
            settings.SetValue(SettingsKeys.RenderWhitespaces, true, false);
            settings.SetValue(SettingsKeys.EnableTimeline, false, false);
            settings.SetValue(SettingsKeys.StartingView, ViewType.Console, false);
            settings.SetValue(SettingsKeys.ClearStdinBufferOnRequest, false, false);
            settings.SetValue(SettingsKeys.ShowPBrainButtons, true, false);
            settings.SetValue(SettingsKeys.OverflowMode, OverflowMode.ByteWithOverflow, false);
            settings.SetValue(SettingsKeys.MemorySize, 128, false);
        }
    }
}
