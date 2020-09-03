using System.Windows.Input;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.Messages.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls
{
    public sealed class VirtualKeyboardViewModel : ObservableRecipient
    {
        /// <summary>
        /// Creates a new <see cref="VirtualKeyboardViewModel"/> instance
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
        public VirtualKeyboardViewModel(ISettingsService settingsService)
        {
            _IsPBrainModeEnabled = settingsService.GetValue<bool>(SettingsKeys.ShowPBrainButtons);

            InsertOperatorCommand = new RelayCommand<char>(InsertOperator);

            Messenger.Register<ShowPBrainButtonsSettingsChangedMessage>(this, m => IsPBrainModeEnabled = m.Value);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for inserting a new Brainf*ck/PBrain operator
        /// </summary>
        public ICommand InsertOperatorCommand { get; }

        private bool _IsPBrainModeEnabled;

        /// <summary>
        /// Gets whether or not the PBrain mode is currently enabled
        /// </summary>
        public bool IsPBrainModeEnabled
        {
            get => _IsPBrainModeEnabled;
            private set => SetProperty(ref _IsPBrainModeEnabled, value);
        }

        /// <summary>
        /// Signals the insertion of a new operator
        /// </summary>
        /// <param name="op">The input Brainf*ck/PBrain operator</param>
        private void InsertOperator(char op)
        {
            Messenger.Send(new OperatorKeyPressedNotificationMessage(op));
        }
    }
}
