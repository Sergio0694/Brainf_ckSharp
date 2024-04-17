using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings;

/// <summary>
/// A viewmodel for the settings page.
/// </summary>
public sealed class SettingsSubPageViewModel : ObservableRecipient
{
    /// <summary>
    /// Creates a new <see cref="SettingsSubPageViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="analyticsService">The <see cref="IAnalyticsService"/> instance to use</param>
    /// <param name="storeService">The <see cref="IStoreService"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    /// <param name="configuration">The <see cref="AppConfiguration"/> instance to use</param>
    public SettingsSubPageViewModel(IMessenger messenger, IAnalyticsService analyticsService, IStoreService storeService, ISettingsService settingsService, AppConfiguration configuration)
        : base(messenger)
    {
        _ = Source.AddGroup(SettingsSection.Ide, new[] { new IdeSettingsSectionViewModel(messenger, analyticsService, storeService, settingsService, configuration) });
        _ = Source.AddGroup(SettingsSection.UI, new[] { new UISettingsSectionViewModel(messenger, settingsService) });
        _ = Source.AddGroup(SettingsSection.Interpreter, new[] { new InterpreterSettingsSectionViewModel(messenger, settingsService) });
    }

    /// <summary>
    /// Gets the current collection of sections to display
    /// </summary>
    public ObservableGroupedCollection<SettingsSection, SettingsSectionViewModelBase> Source { get; } = [];
}
