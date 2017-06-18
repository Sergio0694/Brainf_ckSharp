using System;
using Brainf_ck_sharp_UWP.Messages.Actions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            Messenger.Default.Register<AvailableActionStatusChangedMessage>(this, ProcessConsoleActionsStatusChangedMessage);
            Messenger.Default.Register<IDEExecutableStatusChangedMessage>(this, m => IDECodeAvailable = m.Executable);
            Messenger.Default.Register< DebugStatusChangedMessage>(this, m =>
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
        }

        // Enables or disables the console buttons when needed
        private void ProcessConsoleActionsStatusChangedMessage(AvailableActionStatusChangedMessage message)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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

        private bool _UndoAvailable;

        /// <summary>
        /// Gets whether or not the undo function is enabled
        /// </summary>
        public bool UndoAvailable
        {
            get => _UndoAvailable;
            private set => Set(ref _UndoAvailable, value);
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
    }
}
