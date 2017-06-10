﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.Messages.Actions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            Messenger.Default.Register<ConsoleAvailableActionStatusChangedMessage>(this, ProcessConsoleActionsStatusChangedMessage);
        }

        // Enables or disables the console buttons when needed
        private void ProcessConsoleActionsStatusChangedMessage(ConsoleAvailableActionStatusChangedMessage message)
        {
            switch (message.Action)
            {
                case ConsoleAction.Play:
                    PlayAvailable = message.Status;
                    break;
                case ConsoleAction.Restart:
                    RestartAvailable = message.Status;
                    break;
                case ConsoleAction.Clear:
                    ClearAvailable = message.Status;
                    break;
                case ConsoleAction.Undo:
                    UndoAvailable = message.Status;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
    }
}
