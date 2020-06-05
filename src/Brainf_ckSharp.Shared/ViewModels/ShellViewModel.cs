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
        public void RunConsoleScript() => Messenger.Send<RunCommandRequestMessage>();

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
        public void ClearConsoleScreen() => Messenger.Send<ClearConsoleScreenRequestMessage>();

        /// <summary>
        /// Repeats the last console script
        /// </summary>
        public void RepeatLastConsoleScript() => Messenger.Send<RepeatCommandRequestMessage>();

        /// <summary>
        /// Restarts the console and resets its state
        /// </summary>
        public void RestartConsole() => Messenger.Send<RestartConsoleRequestMessage>();

        /// <summary>
        /// Runs the current IDE script
        /// </summary>
        public void RunIdeScript() => Messenger.Send<RunIdeScriptRequestMessage>();

        /// <summary>
        /// Debugs the current IDE script
        /// </summary>
        public void DebugIdeScript() => Messenger.Send<DebugIdeScriptRequestMessage>();

        /// <summary>
        /// Creates a new file in the IDE
        /// </summary>
        public void NewIdeFile() => Messenger.Send<NewFileRequestMessage>();

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
        public void OpenFile() => Messenger.Send(new PickOpenFileRequestMessage(false));

        /// <summary>
        /// Saves the current source code in the IDE to a file
        /// </summary>
        public void SaveFile() => Messenger.Send<SaveFileRequestMessage>();

        /// <summary>
        /// Saves the current source code in the IDE to a new file
        /// </summary>
        public void SaveFileAs() => Messenger.Send<SaveFileAsRequestMessage>();
    }
}
