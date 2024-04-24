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
    private readonly ISettingsService settingsService;

    /// <summary>
    /// The <see cref="IAnalyticsService"/> instance currently in use
    /// </summary>
    private readonly IAnalyticsService analyticsService;

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
        this.settingsService = settingsService;
        this.analyticsService = analyticsService;

        this.selectedView = this.settingsService.GetValue<ViewType>(SettingsKeys.SelectedView);
        this.isVirtualKeyboardEnabled = this.settingsService.GetValue<bool>(SettingsKeys.IsVirtualKeyboardEnabled);
    }

    private ViewType selectedView;

    /// <summary>
    /// Gets or sets the currently selected view
    /// </summary>
    public ViewType SelectedView
    {
        get => this.selectedView;
        set
        {
            if (SetProperty(ref this.selectedView, value))
            {
                this.settingsService.SetValue(SettingsKeys.SelectedView, value);
            }
        }
    }

    private bool isVirtualKeyboardEnabled;

    /// <summary>
    /// Gets or sets whether or not the virtual keyboard is currently enabled
    /// </summary>
    public bool IsVirtualKeyboardEnabled
    {
        get => this.isVirtualKeyboardEnabled;
        set
        {
            if (SetProperty(ref this.isVirtualKeyboardEnabled, value))
            {
                this.settingsService.SetValue(SettingsKeys.IsVirtualKeyboardEnabled, value);
            }
        }
    }

    /// <summary>
    /// Runs the current console script
    /// </summary>
    public void RunConsoleScript()
    {
        this.analyticsService.Log(EventNames.ConsoleRun);

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
        this.analyticsService.Log(EventNames.ClearScreen);

        _ = Messenger.Send<ClearConsoleScreenRequestMessage>();
    }

    /// <summary>
    /// Repeats the last console script
    /// </summary>
    public void RepeatLastConsoleScript()
    {
        this.analyticsService.Log(EventNames.RepeatLastScript);

        _ = Messenger.Send<RepeatCommandRequestMessage>();
    }

    /// <summary>
    /// Restarts the console and resets its state
    /// </summary>
    public void RestartConsole()
    {
        this.analyticsService.Log(EventNames.Restart);

        _ = Messenger.Send<RestartConsoleRequestMessage>();
    }

    /// <summary>
    /// Runs the current IDE script
    /// </summary>
    public void RunIdeScript()
    {
        this.analyticsService.Log(EventNames.IdeRun);

        _ = Messenger.Send<RunIdeScriptRequestMessage>();
    }

    /// <summary>
    /// Debugs the current IDE script
    /// </summary>
    public void DebugIdeScript()
    {
        this.analyticsService.Log(EventNames.IdeDebug);

        _ = Messenger.Send<DebugIdeScriptRequestMessage>();
    }

    /// <summary>
    /// Creates a new file in the IDE
    /// </summary>
    public void NewIdeFile()
    {
        this.analyticsService.Log(EventNames.NewFile);

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
        this.analyticsService.Log(EventNames.OpenFile);

        _ = Messenger.Send(new PickOpenFileRequestMessage(false));
    }

    /// <summary>
    /// Saves the current source code in the IDE to a file
    /// </summary>
    public void SaveFile()
    {
        this.analyticsService.Log(EventNames.Save);

        _ = Messenger.Send<SaveFileRequestMessage>();
    }

    /// <summary>
    /// Saves the current source code in the IDE to a new file
    /// </summary>
    public void SaveFileAs()
    {
        this.analyticsService.Log(EventNames.SaveAs);

        _ = Messenger.Send<SaveFileAsRequestMessage>();
    }

    /// <summary>
    /// Requests the unicode map
    /// </summary>
    public void RequestUnicodeMap()
    {
        this.analyticsService.Log(EventNames.UnicodeCharactersMapOpened);

        UnicodeMapRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Requests the info about the current app
    /// </summary>
    public void RequestAboutInfo()
    {
        this.analyticsService.Log(EventNames.AboutPageOpened);

        AboutInfoRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Requests the user guide
    /// </summary>
    public void RequestUserGuide()
    {
        this.analyticsService.Log(EventNames.UserGuideOpened);

        UserGuideRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Requests the app settings
    /// </summary>
    public void RequestSettings()
    {
        this.analyticsService.Log(EventNames.SettingsOpened);

        SettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Requests the code library
    /// </summary>
    public void RequestCodeLibrary()
    {
        this.analyticsService.Log(EventNames.OpenCodeLibrary);

        CodeLibraryRequested?.Invoke(this, EventArgs.Empty);
    }
}
