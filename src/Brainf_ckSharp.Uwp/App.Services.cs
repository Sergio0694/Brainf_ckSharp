using System;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Services.Uwp.Analytics;
using Brainf_ckSharp.Services.Uwp.Email;
using Brainf_ckSharp.Services.Uwp.Store;
using Brainf_ckSharp.Services.Uwp.SystemInformation;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.ViewModels;
using Brainf_ckSharp.Shared.ViewModels.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Views;
using Brainf_ckSharp.Uwp.Services.Clipboard;
using Brainf_ckSharp.Uwp.Services.Files;
using Brainf_ckSharp.Uwp.Services.Keyboard;
using Brainf_ckSharp.Uwp.Services.Settings;
using Brainf_ckSharp.Uwp.Services.Share;
using CommunityToolkit.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using GitHub;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Brainf_ckSharp.Uwp;

/// <inheritdoc/>
partial class App
{
    /// <summary>
    /// Builds the <see cref="IServiceProvider"/> instance to use in the application.
    /// </summary>
    /// <returns>The resulting <see cref="IServiceProvider"/> instance.</returns>
    /// <remarks>This method should only be called once during startup.</remarks>
    private IServiceProvider BuildServiceProvider()
    {
        ServiceCollection services = new();

        // Manually register services that need manual control. That is:
        //   - AppConfiguration, as it requires known product ids
        //   - GitHubService, as it requires the user agent to be set
        //   - IFilesManagerService, as the app instance implements it
        _ = services.AddSingleton(new AppConfiguration() { UnlockThemesIapId = "9P4Q63CCFPBM" });
        _ = services.AddSingleton<IGitHubService>(_ => new GitHubService("Brainf_ckSharp|Uwp"));
        _ = services.AddSingleton<IFilesManagerService>(this);

        // Register all other services (this will invoke the source generated stubs)
        ConfigureServices(services);

        // Build the service provider to use in the application
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Performs additional settings configuration and other startup initialization.
    /// </summary>
    /// <param name="serviceProvider">The input service provider to use.</param>
    private void InitializeServices(IServiceProvider serviceProvider)
    {
        ISettingsService settings = serviceProvider.GetRequiredService<ISettingsService>();

        // Initialize default settings
        settings.SetValue(SettingsKeys.IsVirtualKeyboardEnabled, true, false);
        settings.SetValue(SettingsKeys.BracketsFormattingStyle, BracketsFormattingStyle.NewLine, false);
        settings.SetValue(SettingsKeys.IdeTheme, IdeTheme.VisualStudio, false);
        settings.SetValue(SettingsKeys.RenderWhitespaces, true, false);
        settings.SetValue(SettingsKeys.SelectedView, ViewType.Console, false);
        settings.SetValue(SettingsKeys.ClearStdinBufferOnRequest, false, false);
        settings.SetValue(SettingsKeys.ShowPBrainButtons, true, false);
        settings.SetValue(SettingsKeys.DataType, DataType.Byte, false);
        settings.SetValue(SettingsKeys.ExecutionOptions, ExecutionOptions.AllowOverflow, false);
        settings.SetValue(SettingsKeys.MemorySize, 128, false);
    }

    /// <summary>
    /// Configures all services used by the app.
    /// </summary>
    /// <param name="services">The target <see cref="IServiceCollection"/> instance to register services with.</param>
    [Singleton(typeof(WeakReferenceMessenger), typeof(IMessenger))]
    [Singleton(typeof(FilesService), typeof(IFilesService))]
    [Singleton(typeof(SettingsService), typeof(ISettingsService))]
    [Singleton(typeof(KeyboardListenerService), typeof(IKeyboardListenerService))]
    [Singleton(typeof(ClipboardService), typeof(IClipboardService))]
    [Singleton(typeof(ShareService), typeof(IShareService))]
    [Singleton(typeof(EmailService), typeof(IEmailService))]
    [Singleton(typeof(SystemInformationService), typeof(ISystemInformationService))]
#if DEBUG
    [Singleton(typeof(TestStoreService), typeof(IStoreService))]
    [Singleton(typeof(TestAnalyticsService), typeof(IAnalyticsService))]
#else
    [Singleton(typeof(ProductionStoreService), typeof(IStoreService))]
    [Singleton(typeof(ReleaseAnalyticsService), typeof(IAnalyticsService))]
#endif
    [Singleton(typeof(ShellViewModel))]
    [Singleton(typeof(CompactMemoryViewerViewModel))]
    [Singleton(typeof(ConsoleViewModel))]
    [Singleton(typeof(IdeViewModel))]
    [Singleton(typeof(VirtualKeyboardViewModel))]
    [Singleton(typeof(StdinHeaderViewModel))]
    [Singleton(typeof(StatusBarViewModel))]
    [Transient(typeof(SettingsSubPageViewModel))]
    [Transient(typeof(ReviewPromptSubPageViewModel))]
    [Transient(typeof(IdeResultSubPageViewModel))]
    [Transient(typeof(CodeLibrarySubPageViewModel))]
    [Transient(typeof(AboutSubPageViewModel))]
    private static partial void ConfigureServices(IServiceCollection services);
}
