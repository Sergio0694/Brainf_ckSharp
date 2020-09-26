using System;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.Messages.Console.Commands;
using Brainf_ckSharp.Shared.Messages.Ide;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels
{
    /// <summary>
    /// A view model for the root shell control
    /// </summary>
    public sealed class ShellViewModel : ObservableRecipient
    {
        /// <summary>
        /// The <see cref="ISettingsService"/> instance currently in use
        /// </summary>
        private readonly ISettingsService SettingsService;

        /// <summary>
        /// The <see cref="IAnalyticsService"/> instance currently in use
        /// </summary>
        private readonly IAnalyticsService AnalyticsService;

        /// <summary>
        /// Raised whenever the user guide is requested
        /// </summary>
        public event EventHandler? UserGuideRequested;

        /// <summary>
        /// Raised whenever the unicode map is requested
        /// </summary>
        public event EventHandler? UnicodeMapRequested;

        /// <summary>
        /// Raised whenever the settings are requested
        /// </summary>
        public event EventHandler? SettingsRequested;

        /// <summary>
        /// Raised whenever the about info are requested
        /// </summary>
        public event EventHandler? AboutInfoRequested;

        /// <summary>
        /// Raised whenever the code library is requested
        /// </summary>
        public event EventHandler? CodeLibraryRequested;

        /// <summary>
        /// Creates a new <see cref="ShellViewModel"/> instance
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
        /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
        /// <param name="analyticsService">The <see cref="IAnalyticsService"/> instance to use</param>
        public ShellViewModel(IMessenger messenger, ISettingsService settingsService, IAnalyticsService analyticsService)
            : base(messenger)
        {
            SettingsService = settingsService;
            AnalyticsService = analyticsService;

            _SelectedView = SettingsService.GetValue<ViewType>(SettingsKeys.SelectedView);
            _IsVirtualKeyboardEnabled = SettingsService.GetValue<bool>(SettingsKeys.IsVirtualKeyboardEnabled);
        }

        private ViewType _SelectedView;

        /// <summary>
        /// Gets or sets the currently selected view
        /// </summary>
        public ViewType SelectedView
        {
            get => _SelectedView;
            set
            {
                if (SetProperty(ref _SelectedView, value))
                {
                    SettingsService.SetValue(SettingsKeys.SelectedView, value);
                }
            }
        }

        private bool _IsVirtualKeyboardEnabled;

        /// <summary>
        /// Gets or sets whether or not the virtual keyboard is currently enabled
        /// </summary>
        public bool IsVirtualKeyboardEnabled
        {
            get => _IsVirtualKeyboardEnabled;
            set
            {
                if (SetProperty(ref _IsVirtualKeyboardEnabled, value))
                {
                    SettingsService.SetValue(SettingsKeys.IsVirtualKeyboardEnabled, value);
                }
            }
        }

        /// <summary>
        /// Runs the current console script
        /// </summary>
        public void RunConsoleScript()
        {
            AnalyticsService.Log(EventNames.ConsoleRun);

            Messenger.Send<RunCommandRequestMessage>();
        }

        /// <summary>
        /// Deletes the last operator in the current console script
        /// </summary>
        public void DeleteConsoleOperator() => Messenger.Send<DeleteOperatorRequestMessage>();

        /// <summary>
        /// Clears the current console command
        /// </summary>
        public void ClearConsoleCommand() => Messenger.Send<ClearCommandRequestMessage>();

        /// <summary>
        /// Clears the current console screen
        /// </summary>
        public void ClearConsoleScreen()
        {
            AnalyticsService.Log(EventNames.ClearScreen);

            Messenger.Send<ClearConsoleScreenRequestMessage>();
        }

        /// <summary>
        /// Repeats the last console script
        /// </summary>
        public void RepeatLastConsoleScript()
        {
            AnalyticsService.Log(EventNames.RepeatLastScript);

            Messenger.Send<RepeatCommandRequestMessage>();
        }

        /// <summary>
        /// Restarts the console and resets its state
        /// </summary>
        public void RestartConsole()
        {
            AnalyticsService.Log(EventNames.Restart);

            Messenger.Send<RestartConsoleRequestMessage>();
        }

        /// <summary>
        /// Runs the current IDE script
        /// </summary>
        public void RunIdeScript()
        {
            AnalyticsService.Log(EventNames.IdeRun);

            Messenger.Send<RunIdeScriptRequestMessage>();
        }

        /// <summary>
        /// Debugs the current IDE script
        /// </summary>
        public void DebugIdeScript()
        {
            AnalyticsService.Log(EventNames.IdeDebug);

            Messenger.Send<DebugIdeScriptRequestMessage>();
        }

        /// <summary>
        /// Creates a new file in the IDE
        /// </summary>
        public void NewIdeFile()
        {
            AnalyticsService.Log(EventNames.NewFile);

            Messenger.Send<NewFileRequestMessage>();
        }

        /// <summary>
        /// Inserts a new line into the IDE
        /// </summary>
        public void InsertNewLine() => Messenger.Send<InsertNewLineRequestMessage>();

        /// <summary>
        /// Deletes the last character in the IDE
        /// </summary>
        public void DeleteIdeCharacter() => Messenger.Send<DeleteCharacterRequestMessage>();

        /// <summary>
        /// Opens a new file in the IDE
        /// </summary>
        public void OpenFile()
        {
            AnalyticsService.Log(EventNames.OpenFile);

            Messenger.Send(new PickOpenFileRequestMessage(false));
        }

        /// <summary>
        /// Saves the current source code in the IDE to a file
        /// </summary>
        public void SaveFile()
        {
            AnalyticsService.Log(EventNames.Save);

            Messenger.Send<SaveFileRequestMessage>();
        }

        /// <summary>
        /// Saves the current source code in the IDE to a new file
        /// </summary>
        public void SaveFileAs()
        {
            AnalyticsService.Log(EventNames.SaveAs);

            Messenger.Send<SaveFileAsRequestMessage>();
        }

        /// <summary>
        /// Requests the unicode map
        /// </summary>
        public void RequestUnicodeMap()
        {
            AnalyticsService.Log(EventNames.UnicodeCharactersMapOpened);

            UnicodeMapRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Requests the info about the current app
        /// </summary>
        public void RequestAboutInfo()
        {
            AnalyticsService.Log(EventNames.AboutPageOpened);

            AboutInfoRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Requests the user guide
        /// </summary>
        public void RequestUserGuide()
        {
            AnalyticsService.Log(EventNames.UserGuideOpened);

            UserGuideRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Requests the app settings
        /// </summary>
        public void RequestSettings()
        {
            AnalyticsService.Log(EventNames.SettingsOpened);

            SettingsRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Requests the code library
        /// </summary>
        public void RequestCodeLibrary()
        {
            AnalyticsService.Log(EventNames.OpenCodeLibrary);

            CodeLibraryRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
