using System;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Messages.Console.Commands;
using Brainf_ckSharp.Shared.Messages.Ide;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels
{
    /// <summary>
    /// A view model for the root shell control
    /// </summary>
    public sealed class ShellViewModel : ViewModelBase
    {
        /// <summary>
        /// The <see cref="IAnalyticsService"/> instance currently in use
        /// </summary>
        private readonly IAnalyticsService AnalyticsService = Ioc.Default.GetRequiredService<IAnalyticsService>();

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

        private bool _IsVirtualKeyboardEnabled = Ioc.Default.GetRequiredService<ISettingsService>().GetValue<bool>(SettingsKeys.IsVirtualKeyboardEnabled);

        /// <summary>
        /// Gets or sets whether or not the virtual keyboard is currently enabled
        /// </summary>
        public bool IsVirtualKeyboardEnabled
        {
            get => _IsVirtualKeyboardEnabled;
            set
            {
                if (Set(ref _IsVirtualKeyboardEnabled, value))
                {
                    Ioc.Default.GetRequiredService<ISettingsService>().SetValue(SettingsKeys.IsVirtualKeyboardEnabled, value);
                }
            }
        }

        /// <summary>
        /// Runs the current console script
        /// </summary>
        public void RunConsoleScript()
        {
            AnalyticsService.Log(Constants.Events.ConsoleRun);

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
            AnalyticsService.Log(Constants.Events.ClearScreen);

            Messenger.Send<ClearConsoleScreenRequestMessage>();
        }

        /// <summary>
        /// Repeats the last console script
        /// </summary>
        public void RepeatLastConsoleScript()
        {
            AnalyticsService.Log(Constants.Events.RepeatLastScript);

            Messenger.Send<RepeatCommandRequestMessage>();
        }

        /// <summary>
        /// Restarts the console and resets its state
        /// </summary>
        public void RestartConsole()
        {
            AnalyticsService.Log(Constants.Events.Restart);

            Messenger.Send<RestartConsoleRequestMessage>();
        }

        /// <summary>
        /// Runs the current IDE script
        /// </summary>
        public void RunIdeScript()
        {
            AnalyticsService.Log(Constants.Events.IdeRun);

            Messenger.Send<RunIdeScriptRequestMessage>();
        }

        /// <summary>
        /// Debugs the current IDE script
        /// </summary>
        public void DebugIdeScript()
        {
            AnalyticsService.Log(Constants.Events.IdeDebug);

            Messenger.Send<DebugIdeScriptRequestMessage>();
        }

        /// <summary>
        /// Creates a new file in the IDE
        /// </summary>
        public void NewIdeFile()
        {
            AnalyticsService.Log(Constants.Events.NewFile);

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
            AnalyticsService.Log(Constants.Events.OpenFile);

            Messenger.Send(new PickOpenFileRequestMessage(false));
        }

        /// <summary>
        /// Saves the current source code in the IDE to a file
        /// </summary>
        public void SaveFile()
        {
            AnalyticsService.Log(Constants.Events.Save);

            Messenger.Send<SaveFileRequestMessage>();
        }

        /// <summary>
        /// Saves the current source code in the IDE to a new file
        /// </summary>
        public void SaveFileAs()
        {
            AnalyticsService.Log(Constants.Events.SaveAs);

            Messenger.Send<SaveFileAsRequestMessage>();
        }

        /// <summary>
        /// Requests the unicode map
        /// </summary>
        public void RequestUnicodeMap()
        {
            Ioc.Default.GetRequiredService<IAnalyticsService>().Log(Constants.Events.UnicodeCharactersMapOpened);

            UnicodeMapRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Requests the info about the current app
        /// </summary>
        public void RequestAboutInfo()
        {
            Ioc.Default.GetRequiredService<IAnalyticsService>().Log(Constants.Events.AboutPageOpened);

            AboutInfoRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Requests the user guide
        /// </summary>
        public void RequestUserGuide()
        {
            Ioc.Default.GetRequiredService<IAnalyticsService>().Log(Constants.Events.UserGuideOpened);

            UserGuideRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Requests the app settings
        /// </summary>
        public void RequestSettings()
        {
            Ioc.Default.GetRequiredService<IAnalyticsService>().Log(Constants.Events.SettingsOpened);

            SettingsRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Requests the code library
        /// </summary>
        public void RequestCodeLibrary()
        {
            Ioc.Default.GetRequiredService<IAnalyticsService>().Log(Constants.Events.OpenCodeLibrary);

            CodeLibraryRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
