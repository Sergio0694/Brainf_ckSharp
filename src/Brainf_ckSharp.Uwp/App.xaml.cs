﻿using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
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
using Brainf_ckSharp.Services.Uwp.Store;
using Brainf_ckSharp.Services.Uwp.SystemInformation;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.ViewModels;
using Brainf_ckSharp.Shared.ViewModels.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Helpers;

#nullable enable

namespace Brainf_ckSharp.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// The currently requested file to open
        /// </summary>
        private IFile? _RequestedFile;

        /// <summary>
        /// Gets whether or not there is a file request pending
        /// </summary>
        public bool IsFileRequestPending => !(_RequestedFile is null);

        /// <summary>
        /// Extracts the current file request, if present
        /// </summary>
        /// <param name="file">The resulting requested file, if available</param>
        /// <returns>Whether or not a requested file was present</returns>
        public bool TryExtractRequestedFile(out IFile? file)
        {
            file = _RequestedFile;

            _RequestedFile = null;

            return !(file is null);
        }

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            SystemInformation.TrackAppUse(e);

            OnActivated(e.PrelaunchActivated);

            base.OnLaunched(e);
        }

        /// <inheritdoc/>
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            if (args.Files.FirstOrDefault() is StorageFile file)
            {
                _RequestedFile = new File(file);
            }
            else _RequestedFile = null;

            OnActivated(false);

            Services.GetRequiredService<IAnalyticsService>().Log(EventNames.OnFileActivated);

            base.OnFileActivated(args);
        }

        /// <inheritdoc/>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            Uri? uri = (args as ProtocolActivatedEventArgs)?.Uri;

            if (!(uri is null))
            {
                // The app is activated only in two cases: either from a /switch protocol
                // to focus an instance with a requested file already loaded from the IDE, or
                // from a /file protocol to open a file in a new instance. We have the following:
                //   - [/switch] is guaranteed to have the UI already loaded and the target file
                //     already displayed. In this case we just need to focus the IDE page.
                //   - [/file] is guaranteed to only be used for a new instance. This is because
                //     when trying to activate on a target file that is already open, we just
                //     retrieve that instance and redirect the activation to it.
                // First try to get the target file, if present
                if (uri.LocalPath.Equals("/file"))
                {
                    string
                        escapedPath = uri.Query.Substring("?path=".Length),
                        unescapedPath = Uri.UnescapeDataString(escapedPath);

                    StorageFile file = await StorageFile.GetFileFromPathAsync(unescapedPath);

                    _RequestedFile = new File(file);
                }

                // Then only if this is not a new app instance, focus the IDE
                if (Window.Current.Content is Shell shell)
                {
                    shell.BringIdeIntoView();
                }
            }

            OnActivated(false);

            Services.GetRequiredService<IAnalyticsService>().Log(
                EventNames.OnActivated,
                (nameof(Uri.LocalPath), uri?.LocalPath ?? "<NULL>"));

            base.OnActivated(args);
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
                InitializeServices();

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

            await Services.GetRequiredService<IdeViewModel>().SaveStateAsync();

            RegisterFile(null);

            deferral.Complete();
        }

        /// <summary>
        /// Configures the services to use in the app
        /// </summary>
        /// <returns>An <see cref="IServiceProvider"/> instance with the app services.</returns>
        [Pure]
        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Platform specific options
            services.AddOptions<AppConfiguration>().Configure(options => options.UnlockThemesIapId = "9P4Q63CCFPBM");

            // Services
            services.AddSingleton<IFilesService, FilesService>();
            services.AddSingleton<IFilesManagerService>(this);
            services.AddSingleton<IFilesHistoryService, TimelineService>();
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
            // Viewmodels
            services.AddTransient<VirtualKeyboardViewModel>();
            services.AddTransient<StdinHeaderViewModel>();
            services.AddTransient<StatusBarViewModel>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<SettingsSubPageViewModel>();
            services.AddTransient<ReviewPromptSubPageViewModel>();
            services.AddTransient<IdeViewModel>();
            services.AddTransient<IdeResultSubPageViewModel>();
            services.AddTransient<ConsoleViewModel>();
            services.AddTransient<CodeLibrarySubPageViewModel>();
            services.AddTransient<AboutSubPageViewModel>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Performs additional settings configuration and other startup initialization
        /// </summary>
        private void InitializeServices()
        {
            // Initialize the analytics service
            string appCenterSecret = Assembly.GetExecutingAssembly().GetManifestResourceString("Brainf_ckSharp.Uwp.Assets.ServiceTokens.AppCenter.txt");

            Services.GetRequiredService<IAnalyticsService>().Initialize(appCenterSecret);

            ISettingsService settings = Services.GetRequiredService<ISettingsService>();

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
