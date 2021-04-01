﻿using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings
{
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
            Source.AddGroup(SettingsSection.Ide, new IdeSettingsSectionViewModel(messenger, analyticsService, storeService, settingsService, configuration));
            Source.AddGroup(SettingsSection.UI, new UISettingsSectionViewModel(messenger, settingsService));
            Source.AddGroup(SettingsSection.Interpreter, new InterpreterSettingsSectionViewModel(messenger, settingsService));
        }

        /// <summary>
        /// Gets the current collection of sections to display
        /// </summary>
        public ObservableGroupedCollection<SettingsSection, SettingsSectionViewModelBase> Source { get; } = new();
    }
}
