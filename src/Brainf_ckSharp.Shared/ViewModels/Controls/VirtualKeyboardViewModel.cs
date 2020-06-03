using System.Windows.Input;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls
{
    public sealed class VirtualKeyboardViewModel : ViewModelBase
    {
        /// <summary>
        /// Creates a new <see cref="VirtualKeyboardViewModel"/> instance
        /// </summary>
        public VirtualKeyboardViewModel()
        {
            InsertOperatorCommand = new RelayCommand<char>(InsertOperator);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for inserting a new Brainf*ck/PBrain operator
        /// </summary>
        public ICommand InsertOperatorCommand { get; }

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
