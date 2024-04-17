using System;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.Messages.Console.Commands;
using Brainf_ckSharp.Shared.Messages.Ide;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels;

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
        this.SettingsService = settingsService;
        this.AnalyticsService = analyticsService;

        this._SelectedView = this.SettingsService.GetValue<ViewType>(SettingsKeys.SelectedView);
        this._IsVirtualKeyboardEnabled = this.SettingsService.GetValue<bool>(SettingsKeys.IsVirtualKeyboardEnabled);
    }

    private ViewType _SelectedView;

    /// <summary>
    /// Gets or sets the currently selected view
    /// </summary>
    public ViewType SelectedView
    {
        get => this._SelectedView;
        set
        {
            if (SetProperty(ref this._SelectedView, value))
            {
                this.SettingsService.SetValue(SettingsKeys.SelectedView, value);
            }
        }
    }

    private bool _IsVirtualKeyboardEnabled;

    /// <summary>
    /// Gets or sets whether or not the virtual keyboard is currently enabled
    /// </summary>
    public bool IsVirtualKeyboardEnabled
    {
        get => this._IsVirtualKeyboardEnabled;
        set
        {
            if (SetProperty(ref this._IsVirtualKeyboardEnabled, value))
            {
                this.SettingsService.SetValue(SettingsKeys.IsVirtualKeyboardEnabled, value);
            }
        }
    }

    /// <summary>
    /// Runs the current console script
    /// </summary>
    public void RunConsoleScript()
    {
        this.AnalyticsService.Log(EventNames.ConsoleRun);

        _ = Messenger.Send<RunCommandRequestMessage>();
    }

    /// <summary>
    /// Deletes the last operator in the current console script
    /// </summary>
    public void DeleteConsoleOperator()
    {
        _ = Messenger.Send<DeleteOperatorRequestMessage>();
    }

    /// <summary>
    /// Clears the current console command
    /// </summary>
    public void ClearConsoleCommand()
    {
        _ = Messenger.Send<ClearCommandRequestMessage>();
    }

    /// <summary>
    /// Clears the current console screen
    /// </summary>
    public void ClearConsoleScreen()
    {
        this.AnalyticsService.Log(EventNames.ClearScreen);

        _ = Messenger.Send<ClearConsoleScreenRequestMessage>();
    }

    /// <summary>
    /// Repeats the last console script
    /// </summary>
    public void RepeatLastConsoleScript()
    {
        this.AnalyticsService.Log(EventNames.RepeatLastScript);

        _ = Messenger.Send<RepeatCommandRequestMessage>();
    }

    /// <summary>
    /// Restarts the console and resets its state
    /// </summary>
    public void RestartConsole()
    {
        this.AnalyticsService.Log(EventNames.Restart);

        _ = Messenger.Send<RestartConsoleRequestMessage>();
    }

    /// <summary>
    /// Runs the current IDE script
    /// </summary>
    public void RunIdeScript()
    {
        this.AnalyticsService.Log(EventNames.IdeRun);

        _ = Messenger.Send<RunIdeScriptRequestMessage>();
    }

    /// <summary>
    /// Debugs the current IDE script
    /// </summary>
    public void DebugIdeScript()
    {
        this.AnalyticsService.Log(EventNames.IdeDebug);

        _ = Messenger.Send<DebugIdeScriptRequestMessage>();
    }

    /// <summary>
    /// Creates a new file in the IDE
    /// </summary>
    public void NewIdeFile()
    {
        this.AnalyticsService.Log(EventNames.NewFile);

        _ = Messenger.Send<NewFileRequestMessage>();
    }

    /// <summary>
    /// Inserts a new line into the IDE
    /// </summary>
    public void InsertNewLine()
    {
        _ = Messenger.Send<InsertNewLineRequestMessage>();
    }

    /// <summary>
    /// Deletes the last character in the IDE
    /// </summary>
    public void DeleteIdeCharacter()
    {
        _ = Messenger.Send<DeleteCharacterRequestMessage>();
    }

    /// <summary>
    /// Opens a new file in the IDE
    /// </summary>
    public void OpenFile()
    {
        this.AnalyticsService.Log(EventNames.OpenFile);

        _ = Messenger.Send(new PickOpenFileRequestMessage(false));
    }

    /// <summary>
    /// Saves the current source code in the IDE to a file
    /// </summary>
    public void SaveFile()
    {
        this.AnalyticsService.Log(EventNames.Save);

        _ = Messenger.Send<SaveFileRequestMessage>();
    }

    /// <summary>
    /// Saves the current source code in the IDE to a new file
    /// </summary>
    public void SaveFileAs()
    {
        this.AnalyticsService.Log(EventNames.SaveAs);

        _ = Messenger.Send<SaveFileAsRequestMessage>();
    }

    /// <summary>
    /// Requests the unicode map
    /// </summary>
    public void RequestUnicodeMap()
    {
        this.AnalyticsService.Log(EventNames.UnicodeCharactersMapOpened);

        UnicodeMapRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Requests the info about the current app
    /// </summary>
    public void RequestAboutInfo()
    {
        this.AnalyticsService.Log(EventNames.AboutPageOpened);

        AboutInfoRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Requests the user guide
    /// </summary>
    public void RequestUserGuide()
    {
        this.AnalyticsService.Log(EventNames.UserGuideOpened);

        UserGuideRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Requests the app settings
    /// </summary>
    public void RequestSettings()
    {
        this.AnalyticsService.Log(EventNames.SettingsOpened);

        SettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Requests the code library
    /// </summary>
    public void RequestCodeLibrary()
    {
        this.AnalyticsService.Log(EventNames.OpenCodeLibrary);

        CodeLibraryRequested?.Invoke(this, EventArgs.Empty);
    }
}
