using Brainf_ckSharp.Uwp.Messages.Console.Commands;
using Brainf_ckSharp.Uwp.Messages.Ide;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Uwp.ViewModels
{
    /// <summary>
    /// A view model for the root shell control
    /// </summary>
    public sealed class ShellViewModel
    {
        /// <summary>
        /// Runs the current console script
        /// </summary>
        public void RunConsoleScript() => Messenger.Default.Send<RunCommandRequestMessage>();

        /// <summary>
        /// Deletes the last operator in the current console script
        /// </summary>
        public void DeleteConsoleOperator() => Messenger.Default.Send<DeleteOperatorRequestMessage>();

        /// <summary>
        /// Clears the current console command
        /// </summary>
        public void ClearConsoleCommand() => Messenger.Default.Send<ClearCommandRequestMessage>();

        /// <summary>
        /// Clears the current console screen
        /// </summary>
        public void ClearConsoleScreen() => Messenger.Default.Send<ClearConsoleScreenRequestMessage>();

        /// <summary>
        /// Repeats the last console script
        /// </summary>
        public void RepeatLastConsoleScript() => Messenger.Default.Send<RepeatCommandRequestMessage>();

        /// <summary>
        /// Restarts the console and resets its state
        /// </summary>
        public void RestartConsole() => Messenger.Default.Send<RestartConsoleRequestMessage>();

        /// <summary>
        /// Runs the current IDE script
        /// </summary>
        public void RunIdeScript() => Messenger.Default.Send<RunIdeScriptRequestMessage>();

        /// <summary>
        /// Inserts a new line into the IDE
        /// </summary>
        public void InsertNewLine() => Messenger.Default.Send<InsertNewLineRequestMessage>();

        /// <summary>
        /// Deletes the last character in the IDE
        /// </summary>
        public void DeleteIdeCharacter() => Messenger.Default.Send<DeleteCharacterRequestMessage>();

        /// <summary>
        /// Opens a new file in the IDE
        /// </summary>
        public void OpenFile() => Messenger.Default.Send(new PickOpenFileRequestMessage(false));

        /// <summary>
        /// Saves the current source code in the IDE to a file
        /// </summary>
        public void SaveFile() => Messenger.Default.Send<SaveFileRequestMessage>();

        /// <summary>
        /// Saves the current source code in the IDE to a new file
        /// </summary>
        public void SaveFileAs() => Messenger.Default.Send<SaveFileAsRequestMessage>();
    }
}
