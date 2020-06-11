using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Services.Uwp.Analytics;
using Brainf_ckSharp.Services.Uwp.Email;
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
using Brainf_ckSharp.Services.Uwp.SystemInformation;
using Brainf_ckSharp.Shared;
using Brainf_ckSharp.Shared.Messages.Ide;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using File = Brainf_ckSharp.Uwp.Services.Files.File;
using FileIO = System.IO.File;

#nullable enable

namespace Brainf_ckSharp.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// The entry point for the whole application
        /// </summary>
        public static void Main(string[] args)
        {
            // Check if this invocation already has a target instance (from the shell)
            if (AppInstance.RecommendedInstance is null)
            {
                string key = Guid.NewGuid().ToString("N");

                // If the activation requests a file, check if it is already in use
                if (AppInstance.GetActivatedEventArgs() is FileActivatedEventArgs fileArgs &&
                    fileArgs.Files.FirstOrDefault() is StorageFile storageFile)
                {
                    string temporaryPath = ApplicationData.Current.TemporaryFolder.Path;

                    foreach (AppInstance instance in AppInstance.GetInstances())
                    {
                        // Read the file with the same key as the current instance
                        string keyPath = Path.Combine(temporaryPath, instance.Key);

                        if (!FileIO.Exists(keyPath)) continue;

                        string currentPath = FileIO.ReadAllText(keyPath);

                        // If an active instance is find, just switch to it
                        if (storageFile.Path.Equals(currentPath))
                        {
                            instance.RedirectActivationTo();

                            return;
                        }
                    }
                }

                // This will be a new app instance, which needs to be registered
                _ = AppInstance.FindOrRegisterInstanceForKey(key);

                // Start the app as usual, passing the current key
                Start(_ => new App(key));
            }
            else AppInstance.RecommendedInstance.RedirectActivationTo();
        }

        /// <summary>
        /// Registers the currently opened file for the current application instance
        /// </summary>
        /// <param name="file">The file currently opened, if present</param>
        public void RegisterCurrentFilePath(IFile? file)
        {
            string
                temporaryPath = ApplicationData.Current.TemporaryFolder.Path,
                keyPath = Path.Combine(temporaryPath, Id);

            if (file is null) FileIO.Delete(keyPath);
            else FileIO.WriteAllText(keyPath, file.Path);
        }

        /// <summary>
        /// Creates a new <see cref="App"/> instance
        /// </summary>
        /// <param name="id">The unique id associated with this app instance</param>
        public App(string id)
        {
            Id = id;

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the unique id associated with this app instance
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets or sets the currently requested file to open
        /// </summary>
        public IFile? RequestedFile { get; set; }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            SystemInformation.TrackAppUse(e);

            OnActivated(e.PrelaunchActivated);
        }

        /// <inheritdoc/>
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            if (args.Files.FirstOrDefault() is StorageFile storageFile)
            {
                IFile file = RequestedFile = new File(storageFile);

                Messenger.Default.Send(new OpenFileRequestMessage(file));
            }

            OnActivated(false);

            base.OnFileActivated(args);
        }

        /// <summary>
        /// Initilizes and activates the app
        /// </summary>
        /// <param name="prelaunchActivated">Whether or not the prelaunch is enabled for the current activation</param>
        private void OnActivated(bool prelaunchActivated)
        {
            // Initialize the UI if needed
            if (!(Window.Current.Content is Shell))
            {
                ConfigureServices();

                // Initial UI styling
                TitleBarHelper.ExpandViewIntoTitleBar();
                TitleBarHelper.StyleTitleBar();

                Window.Current.Content = new Shell();

                SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequested;
            }

            // Activate the window when launching the app
            if (prelaunchActivated == false)
            {
                CoreApplication.EnablePrelaunch(true);

                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Handles the closure of the current app
        /// </summary>
        /// <param name="sender">The current application</param>
        /// <param name="e">The <see cref="SystemNavigationCloseRequestedPreviewEventArgs"/> instance for the current event</param>
        private async void OnCloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            Deferral deferral = e.GetDeferral();

            await Messenger.Default.Send<SaveIdeStateRequestMessage>().Result;

            RegisterCurrentFilePath(null);

            deferral.Complete();
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
                services.AddSingleton<IEmailService, EmailService>();
                services.AddSingleton<ISystemInformationService, SystemInformationService>();
                services.AddSingleton(_ => GitHubRestFactory.GetGitHubService("Brainf_ckSharp|Uwp"));
#if DEBUG
                services.AddSingleton<IStoreService, TestStoreService>();
                services.AddSingleton<IAnalyticsService, TestAnalyticsService>();
#else
                services.AddSingleton<IStoreService, ProductionStoreService>();
                services.AddSingleton<IAnalyticsService, AppCenterService>();
#endif
            });

            // Initialize the analytics service
            string appCenterSecret = Assembly.GetExecutingAssembly().ReadTextFromEmbeddedResourceFile("AppCenter.txt");

            Ioc.Default.GetRequiredService<IAnalyticsService>().Initialize(appCenterSecret);

            ISettingsService settings = Ioc.Default.GetRequiredService<ISettingsService>();

            // Initialize default settings
            settings.SetValue(SettingsKeys.IsVirtualKeyboardEnabled, true, false);
            settings.SetValue(SettingsKeys.AutoindentBrackets, true, false);
            settings.SetValue(SettingsKeys.BracketsFormattingStyle, BracketsFormattingStyle.NewLine, false);
            settings.SetValue(SettingsKeys.IdeTheme, IdeTheme.VisualStudio, false);
            settings.SetValue(SettingsKeys.RenderWhitespaces, true, false);
            settings.SetValue(SettingsKeys.SelectedView, ViewType.Console, false);
            settings.SetValue(SettingsKeys.ClearStdinBufferOnRequest, false, false);
            settings.SetValue(SettingsKeys.ShowPBrainButtons, true, false);
            settings.SetValue(SettingsKeys.OverflowMode, OverflowMode.ByteWithOverflow, false);
            settings.SetValue(SettingsKeys.MemorySize, 128, false);
        }
    }
}
