using Brainf_ckSharp.UWP.Controls.SubPages.Shell;
using Brainf_ckSharp.UWP.Controls.SubPages.Shell.UserGuide;
using Brainf_ckSharp.UWP.Messages.Console.Commands;
using Brainf_ckSharp.UWP.Messages.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ckSharp.UWP.ViewModels
{
    /// <summary>
    /// A view model for the root shell control
    /// </summary>
    public sealed class ShellViewModel : ViewModelBase
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
        /// Shows the info about the current app
        /// </summary>
        public void ShowAppInfo() => Messenger.Default.Send(SubPageNavigationRequestMessage.To<AboutSubPage>());

        /// <summary>
        /// Shows the user guide
        /// </summary>
        public void ShowUserGuide() => Messenger.Default.Send(SubPageNavigationRequestMessage.To<UserGuideSubPage>());
    }
}
