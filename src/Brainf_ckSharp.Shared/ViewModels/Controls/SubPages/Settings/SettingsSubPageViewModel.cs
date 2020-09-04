using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings
{
    public sealed class SettingsSubPageViewModel : ObservableRecipient
    {
        /// <summary>
        /// Creates a new <see cref="SettingsSubPageViewModel"/> instance
        /// </summary>
        /// <param name="analyticsService">The <see cref="IAnalyticsService"/> instance to use</param>
        /// <param name="storeService">The <see cref="IStoreService"/> instance to use</param>
        /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
        /// <param name="configuration">The <see cref="IOptions{TOptions}"/> instance to use</param>
        public SettingsSubPageViewModel(IAnalyticsService analyticsService, IStoreService storeService, ISettingsService settingsService, IOptions<AppConfiguration> configuration)
        {
            Source.AddGroup(SettingsSection.Ide, new IdeSettingsSectionViewModel(analyticsService, storeService, settingsService, configuration));
            Source.AddGroup(SettingsSection.UI, new UISettingsSectionViewModel(settingsService));
            Source.AddGroup(SettingsSection.Interpreter, new InterpreterSettingsSectionViewModel(settingsService));
        }

        /// <summary>
        /// Gets the current collection of sections to display
        /// </summary>
        public ObservableGroupedCollection<SettingsSection, SettingsSectionViewModelBase> Source { get; } = new ObservableGroupedCollection<SettingsSection, SettingsSectionViewModelBase>();
    }
}
