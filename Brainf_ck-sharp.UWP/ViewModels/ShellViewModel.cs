using System;
using Brainf_ck_sharp.Enums;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.Messages.Actions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations.Helpers;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        // A function that retrieves the current stdin buffer to use
        [NotNull]
        private readonly Func<String> StdinBufferExtractor;

        public ShellViewModel([NotNull] Func<String> stdinExtractor)
        {
            // General messages
            StdinBufferExtractor = stdinExtractor;
            Messenger.Default.Register<AvailableActionStatusChangedMessage>(this, ProcessAvailableActionsStatusChangedMessage);
            Messenger.Default.Register<IDEExecutableStatusChangedMessage>(this, m => IDECodeAvailable = m.Executable);
            Messenger.Default.Register<DebugStatusChangedMessage>(this, m =>
            {
                if (_DebugAvailable != m.DebugAvailable)
                {
                    _DebugAvailable = m.DebugAvailable;
                    RaisePropertyChanged(() => DebugAvailable);
                }
            });
            Messenger.Default.Register<SaveButtonsEnabledStatusChangedMessage>(this, m =>
            {
                SaveAvailable = m.SaveEnabled;
                SaveAsAvailable = m.SaveAsEnabled;
            });

            // Overflow mode
            Messenger.Default.Register<OverflowModeChangedMessage>(this, m => _OverflowMode = m.Mode);
            AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.ByteOverflowModeEnabled), out bool overflow);
            _OverflowMode = overflow ? OverflowMode.ByteOverflow : OverflowMode.ShortNoOverflow;
        }

        // Enables or disables the console buttons when needed
        private void ProcessAvailableActionsStatusChangedMessage(AvailableActionStatusChangedMessage message)
        {
            switch (message.Action)
            {
                case SharedAction.Play:
                    PlayAvailable = message.Status;
                    break;
                case SharedAction.Restart:
                    RestartAvailable = message.Status;
                    break;
                case SharedAction.Clear:
                    ClearAvailable = message.Status;
                    break;
                case SharedAction.DeleteLastCharacter:
                    UndoAvailable = message.Status;
                    break;
                case SharedAction.ClearScreen:
                    ClearScreenAvailable = message.Status;
                    break;
                case SharedAction.RepeatLastScript:
                    RepeatLastScriptAvailable = message.Status;
                    break;
                case SharedAction.Undo:
                    UndoAvailable = message.Status;
                    break;
                case SharedAction.Redo:
                    RedoAvailable = message.Status;
                    break;
                case SharedAction.Delete:
                    DeleteAvailable = message.Status;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Parameters

        /// <summary>
        /// Gets whether or not the current device is a desktop
        /// </summary>
        public bool DesktopMode => ApiInformationHelper.IsDesktop;

        // The current overflow mode to use
        private OverflowMode _OverflowMode;

        private bool _RepeatLastScriptAvailable;

        /// <summary>
        /// Gets or sets whether or not it is possible to repeat the last script that was run in the console
        /// </summary>
        public bool RepeatLastScriptAvailable
        {
            get => _RepeatLastScriptAvailable;
            private set => Set(ref _RepeatLastScriptAvailable, value);
        }

        private bool _RestartAvailable;

        /// <summary>
        /// Gets or sets whether or not the console can be restarted
        /// </summary>
        public bool RestartAvailable
        {
            get => _RestartAvailable;
            private set => Set(ref _RestartAvailable, value);
        }

        private bool _ClearAvailable;

        /// <summary>
        /// Gets or sets whether or not the last console line can be cleared
        /// </summary>
        public bool ClearAvailable
        {
            get => _ClearAvailable;
            private set => Set(ref _ClearAvailable, value);
        }

        private bool _DeleteAvailable;

        /// <summary>
        /// Gets whether or not the undo delete is enabled in the IDE
        /// </summary>
        public bool DeleteAvailable
        {
            get => _DeleteAvailable;
            private set => Set(ref _DeleteAvailable, value);
        }

        private bool _UndoAvailable;

        /// <summary>
        /// Gets whether or not the undo function is enabled
        /// </summary>
        public bool UndoAvailable
        {
            get => _UndoAvailable;
            private set => Set(ref _UndoAvailable, value);
        }

        private bool _RedoAvailable;

        /// <summary>
        /// Gets whether or not the redo function is enabled
        /// </summary>
        public bool RedoAvailable
        {
            get => _RedoAvailable;
            private set => Set(ref _RedoAvailable, value);
        }

        private bool _PlayAvailable;

        /// <summary>
        /// Gets whether or not there is a script ready to be executed
        /// </summary>
        public bool PlayAvailable
        {
            get => _PlayAvailable;
            private set => Set(ref _PlayAvailable, value);
        }

        private bool _ClearScreenAvailable;

        /// <summary>
        /// Gets whether or not it is possible to clear the screen
        /// </summary>
        public bool ClearScreenAvailable
        {
            get => _ClearScreenAvailable;
            private set => Set(ref _ClearScreenAvailable, value);
        }

        private bool _IDECodeAvailable;

        /// <summary>
        /// Gets or sets whether or not there is code to run in the IDE
        /// </summary>
        public bool IDECodeAvailable
        {
            get => _IDECodeAvailable;
            private set
            {
                if (Set(ref _IDECodeAvailable, value))
                    RaisePropertyChanged(() => DebugAvailable);
            }
        }

        private bool _DebugAvailable;

        /// <summary>
        /// Gets whether or not the IDE debug button is enabled
        /// </summary>
        public bool DebugAvailable => IDECodeAvailable && _DebugAvailable;

        private bool _SaveAvailable;

        /// <summary>
        /// Gets whether or not it is possible to save the current source code
        /// </summary>
        public bool SaveAvailable
        {
            get => _SaveAvailable;
            private set => Set(ref _SaveAvailable, value);
        }

        private bool _SaveAsAvailable;

        /// <summary>
        /// Gets or sets whether or not is possible to save the current code as a new file
        /// </summary>
        public bool SaveAsAvailable
        {
            get => _SaveAsAvailable;
            private set => Set(ref _SaveAsAvailable, value);
        }

        #endregion

        #region Messages forwarding

        // Retrieves the current stdin buffer and then forwards the appropriate message
        private void SendPlayRequestMessage(ScriptPlayType type)
        {
            String stdin = StdinBufferExtractor();
            Messenger.Default.Send(new PlayScriptMessage(type, stdin, _OverflowMode));
        }

        /// <summary>
        /// Sends a play request to the console or the IDE
        /// </summary>
        public void RequestPlay() => SendPlayRequestMessage(ScriptPlayType.Default);

        /// <summary>
        /// Sends a debug request to the IDE, if there is at least an active breakpoint
        /// </summary>
        public void RequestDebug() => SendPlayRequestMessage(ScriptPlayType.Debug);

        /// <summary>
        /// Sends a message to the console to repeat the last available script
        /// </summary>
        public void RequestRepeatLastConsoleScript() => SendPlayRequestMessage(ScriptPlayType.RepeatedCommand);

        /// <summary>
        /// Sends a message to clear the current command in the console
        /// </summary>
        public void RequestClearConsoleLine() => Messenger.Default.Send(new ClearConsoleLineMessage());

        /// <summary>
        /// Sends a message to delete the last character in the current console command
        /// </summary>
        public void RequestUndoConsoleCharacter() => Messenger.Default.Send(new UndoConsoleCharacterMessage());

        /// <summary>
        /// Sends a message to restart the console and reset the current machine state
        /// </summary>
        public void RequestRestartConsole() => Messenger.Default.Send(new RestartConsoleMessage());

        /// <summary>
        /// Sends a message to either the console or the IDE to clear the current content
        /// </summary>
        public void RequestClearScreen() => Messenger.Default.Send(new ClearScreenMessage());

        /// <summary>
        /// Sends a message to the IDE to save the current code that's being edited
        /// </summary>
        public void RequestSaveSourceCode() => Messenger.Default.Send(new SaveSourceCodeRequestMessage(CodeSaveType.Save));

        /// <summary>
        /// Sends a message to the IDE to save the current code as a new document
        /// </summary>
        public void RequestSaveSourceCodeAs() => Messenger.Default.Send(new SaveSourceCodeRequestMessage(CodeSaveType.SaveAs));

        /// <summary>
        /// Sends a message to the IDE to undo the latest change to the code
        /// </summary>
        public void RequestIDEUndo() => Messenger.Default.Send(new IDEUndoRedoRequestMessage(UndoRedoOperation.Undo));

        /// <summary>
        /// Sends a message to the IDE to redo the latest change to the code
        /// </summary>
        public void RequestIDERedo() => Messenger.Default.Send(new IDEUndoRedoRequestMessage(UndoRedoOperation.Redo));

        /// <summary>
        /// Sends a message to the IDE to add a new '\r' character
        /// </summary>
        public void RequestAddIDENewLine() => Messenger.Default.Send(new IDENewLineRequestedMessage());

        /// <summary>
        /// Sends a message to the IDE to delete the current character
        /// </summary>
        public void RequestIDEDeleteAction() => Messenger.Default.Send(new IDEDeleteCharacterRequestMessage());

        #endregion
    }
}
